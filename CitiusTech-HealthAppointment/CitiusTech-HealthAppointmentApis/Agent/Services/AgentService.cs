using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Handler;
using Microsoft.Extensions.Logging;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Services;
using PatientAppointments.Core.Entities;
using System.Diagnostics;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Services
{
    public class AgentService : IAgentService
    {
        private readonly PersistentAgentsClient _client;
        private readonly PersistentAgent _agent;
        private readonly ILogger<AgentService> _logger;
        private readonly IEnumerable<IToolHandler> _toolHandlers;
        private readonly IAgentConversationManager _agentConversationService;
        private readonly IAuthManager _authManager;

        public AgentService(
            PersistentAgentsClient persistentAgentsClient,
            PersistentAgent agent,
            IEnumerable<IToolHandler> toolHandlers,
            IAgentConversationManager agentConversationService,
            ILogger<AgentService> logger,
            IAuthManager authManager
            )
        {
            _client = persistentAgentsClient;
            _agent = agent;
            _toolHandlers = toolHandlers;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _agentConversationService = agentConversationService;
            _authManager = authManager;
        }

        /// <summary>
        /// Resolves a single tool call by matching it with a registered IToolHandler.
        /// </summary>
        public async Task<ToolOutput?> GetResolvedToolOutputAsync(RequiredToolCall toolCall)
        {
            if (toolCall is not RequiredFunctionToolCall functionToolCall)
                return null;

            _logger.LogInformation("Tool invoked: {ToolName} | ID: {ToolId}", functionToolCall.Name, toolCall.Id);
            _logger.LogInformation("Arguments: {Arguments}", functionToolCall.Arguments);

            try
            {
                using var doc = JsonDocument.Parse(functionToolCall.Arguments);
                var root = doc.RootElement;

                var handler = _toolHandlers.FirstOrDefault(h => h.ToolName == functionToolCall.Name);
                if (handler == null)
                {
                    _logger.LogWarning("No handler found for tool: {ToolName}", functionToolCall.Name);
                    return null;
                }

                return await handler.HandleAsync(functionToolCall, root);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while resolving tool output for {ToolName}", functionToolCall.Name);
                return null;
            }
        }
        private int _apiCallCount = 0;

        private async Task<T> CallAzureApiAsync<T>(Func<Task<T>> apiCall, string apiName, string? details = null)
        {
            _apiCallCount++;
            _logger.LogInformation("⛅Azure API call #{Count} - {ApiName} {Details}", _apiCallCount, apiName, details ?? "");

            var stopwatch = Stopwatch.StartNew();
            var result = await apiCall();
            stopwatch.Stop();

            _logger.LogInformation("{ApiName} completed in {ElapsedMilliseconds} ms", apiName, stopwatch.ElapsedMilliseconds);
            _logger.LogInformation("_________________________________________________________________________");
            return result;
        }


        public async Task<MessageContent?> GetAgentResponseAsync(MessageRole role, string message)
        {
            const int maxRetries = 6;
            const int baseDelayMs = 5000;        // start with small delay for polling
            const int maxDelayMs = 30000;        // cap at 3s
            const int submitDelayMs = 100;      // small pause after submitting tools

            // sequence-based backoff: 500ms → 1000ms → 2000ms → 2000ms → ...
            var pollingDelays = new[] { 1000, 2000, 3000, 4000, 5000 };
            int pollIndex = 0;

            var threadId = await _authManager.FetchOrCreateThreadForUser();

            _logger.LogInformation("Sending user message of length {Length}", message.Length);

            try
            {
                // Add user message to thread
                await CallAzureApiAsync(
                    () => _client.Messages.CreateMessageAsync(threadId, MessageRole.User, message),
                    "CreateMessageAsync",
                    $"message length={message.Length}");

                int attempt = 0;
                ThreadRun run = await CallAzureApiAsync(
                    () => _client.Runs.CreateRunAsync(threadId, _agent.Id),
                    "CreateRunAsync",
                    $"agentId={_agent.Id}");


                while (attempt < maxRetries)
                {
                    attempt++;
                    try
                    {
                        //int delayMs = baseDelayMs;
                        bool continuePolling;

                        do
                        {
                            var delayMs = pollIndex < pollingDelays.Length
                                ? pollingDelays[pollIndex]
                                : pollingDelays.Last(); // cap at 2s

                            pollIndex++;
                            await Task.Delay(delayMs);

                            run = await CallAzureApiAsync(
                                () => _client.Runs.GetRunAsync(threadId, run.Id),
                                "GetRunAsync",
                                $"runId={run.Id}");

                            // Increase polling delay exponentially
                            delayMs = Math.Min(delayMs * 2, maxDelayMs);

                            if (run.Status == RunStatus.RequiresAction && run.RequiredAction is SubmitToolOutputsAction action)
                            {
                                // Resolve all tool outputs in parallel if possible
                                var toolOutputsTasks = action.ToolCalls
                                    .Select(toolCall => GetResolvedToolOutputAsync(toolCall))
                                    .ToArray();

                                var toolOutputs = (await Task.WhenAll(toolOutputsTasks))
                                                  .Where(x => x != null)
                                                  .ToList();

                                if (toolOutputs.Any())
                                {
                                    run = await CallAzureApiAsync(
                                        () => _client.Runs.SubmitToolOutputsToRunAsync(threadId, run.Id, toolOutputs),
                                        "SubmitToolOutputsToRunAsync",
                                        $"runId={run.Id}, toolCalls={toolOutputs.Count}");

                                    // await Task.Delay(submitDelayMs); // short pause to avoid burst
                                }
                            }

                            if (run.Status == RunStatus.Failed && run.LastError != null)
                            {
                                if (!string.IsNullOrEmpty(run.LastError.Code) &&
                                    run.LastError.Code.Contains("rate_limit_exceeded", StringComparison.OrdinalIgnoreCase))
                                {
                                    throw new RateLimitExceededException("Rate limit exceeded.");
                                }
                                else
                                {
                                    _logger.LogError("Run failed: {Code} - {Message}", run.LastError.Code, run.LastError.Message);
                                    return null; // unrecoverable
                                }
                            }

                            continuePolling = run.Status == RunStatus.Queued
                                             || run.Status == RunStatus.InProgress
                                             || run.Status == RunStatus.RequiresAction;

                        } while (continuePolling);

                        // Fetch final messages
                        var messages = await CallAzureApiAsync(
                            () => Task.FromResult(_client.Messages.GetMessages(threadId, runId: run.Id, order: ListSortOrder.Descending)),
                            "GetMessages",
                            $"runId={run.Id}");

                        var agentMsg = messages
                            .FirstOrDefault(m => m.Role == MessageRole.Agent)?
                            .ContentItems.OfType<MessageTextContent>()
                            .FirstOrDefault();

                        if (agentMsg != null)
                        {
                            _logger.LogInformation("Returning message content: {Text}", agentMsg.Text);
                            return agentMsg;
                        }

                        _logger.LogWarning("No assistant response found after run completion.");
                        return null;
                    }
                    catch (RateLimitExceededException ex)
                    {
                        if (attempt == maxRetries)
                        {
                            _logger.LogError(ex, "Max retries reached due to rate limit.");
                            throw;
                        }

                        // Exponential retry with jitter
                        var retryDelay = Math.Min(baseDelayMs * (int)Math.Pow(2, attempt), maxDelayMs) + Random.Shared.Next(200, 800);
                        _logger.LogWarning(ex, "Rate limit hit on attempt {Attempt}, retrying after {RetryDelay}ms...", attempt, retryDelay);

                        retryDelay = 5000;
                        await Task.Delay(retryDelay);

                        // Cancel old run before retry
                        if (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress || run.Status == RunStatus.RequiresAction)
                        {
                            try
                            {
                                _logger.LogInformation("Cancelling old run before retry. RunId: {RunId}", run.Id);
                                await CallAzureApiAsync(
                                    () => _client.Runs.CancelRunAsync(threadId, run.Id),
                                    "CancelRunAsync",
                                    $"runId={run.Id}");
                            }
                            catch (Exception cancelEx)
                            {
                                _logger.LogWarning(cancelEx, "Failed to cancel run {RunId}", run.Id);
                            }
                        }

                        // Create a fresh run
                        run = await CallAzureApiAsync(
                            () => _client.Runs.CreateRunAsync(threadId, _agent.Id),
                            "CreateRunAsync",
                            $"agentId={_agent.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Run failed with unrecoverable error.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get agent response.");
                await this.Refresh();
                await this.GetAgentResponseAsync(role, message);
            }
            return null;
        }

        // Custom exception for clarity
        public class RateLimitExceededException : Exception
        {
            public RateLimitExceededException(string message) : base(message) { }
        }
        

        /// <summary>
        /// Creates a new persistent agent thread for communication.
        /// </summary>
        public async Task<string> Refresh()
        {
            // Delete thread for currently logged in user
            await _authManager.DeleteThreadForUserAsync();
            // Create a new thread for the user
            var userId = (await _authManager.GetLoggedInUserInfo()).userId;

            // Create new thread for user
            var threadId = await _authManager.FetchOrCreateThreadForUser(userId);

            return threadId;

        }


        /// <summary>
        /// Adds a user message to the provided thread.
        /// </summary>
        public async Task AddUserMessageAsync(string threadId, MessageRole role, string message)
        {
            _logger.LogInformation($"Adding user message to thread {threadId}: {message}");
            await _client.Messages.CreateMessageAsync(threadId, MessageRole.User, message);
        }       

    }
}

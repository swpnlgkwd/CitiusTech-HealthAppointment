using Azure.AI.Agents.Persistent;

namespace CitiusTech_HealthAppointmentApis.Agent.Services
{
    public interface IAgentService
    {
        Task AddUserMessageAsync(string threadId, MessageRole role, string message);

        Task<MessageContent?> GetAgentResponseAsync(MessageRole role, string message);

        Task<ToolOutput?> GetResolvedToolOutputAsync(RequiredToolCall toolCall);

        Task<string> Refresh();
    }
}

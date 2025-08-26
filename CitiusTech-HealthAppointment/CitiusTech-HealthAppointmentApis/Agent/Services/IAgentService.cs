using Azure.AI.Agents.Persistent;

namespace CitiusTech_HealthAppointmentApis.Agent.Services
{
    public interface IAgentService
    {
        Task<PersistentAgentThread> CreateThreadAsync();

        Task AddUserMessageAsync(string threadId, MessageRole role, string message);

        Task<MessageContent?> GetAgentResponseAsync(MessageRole role, string message);

        Task<ToolOutput?> GetResolvedToolOutputAsync(RequiredToolCall toolCall);

        //Task DeleteThreadForUserAsync();

        Task<string> FetchOrCreateThreadForUser(int? staffId = null);

        //Task<string> Refresh();
    }
}

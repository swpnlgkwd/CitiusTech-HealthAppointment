using Microsoft.AspNetCore.Http;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Services
{
    public class AgentConversationManager : IAgentConversationManager
    {
        private readonly IAgentConversationsRepository agentConversationsRepository;
        private IHttpContextAccessor _httpContextAccessor;
        public AgentConversationManager(IAgentConversationsRepository agentConversationsRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.agentConversationsRepository = agentConversationsRepository;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task AddAgentConversation(AgentConversations agentConversations)
        {
            await agentConversationsRepository.AddAsync(agentConversations);
            await agentConversationsRepository.SaveAsync();
        }

        public async Task DeleteAgentConversation(AgentConversations agentConversation)
        {
            agentConversationsRepository.Remove(agentConversation);
            await agentConversationsRepository.SaveAsync();
        }

        public async Task<AgentConversations?> FetchLoggedInUserAgentConversationInfo()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var agentConversation = (await agentConversationsRepository.GetAllAsync())
                .FirstOrDefault(x => x.user_id == userId?.ToString());

            // May return null if the conversation does not exist
            return agentConversation;
        }

        public async Task<string?> FetchThreadIdForLoggedInUser(string? usrId)
        {
            string? userId;

            if (!string.IsNullOrEmpty(usrId))
            {
                userId = usrId;
            }
            else
            {
                userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var agentConversation = (await agentConversationsRepository.GetAllAsync())
                .FirstOrDefault(x => x.user_id == userId.ToString());

            return agentConversation?.thread_id;
        }

        public async Task UpdateThreadForUser(int userId, string threadId)
        {
            var agentConversation = new AgentConversations
            {
                thread_id = threadId,
                user_id = userId.ToString(),
                created_at = DateTime.Now,
            };
            agentConversationsRepository.Update(agentConversation);
            await agentConversationsRepository.SaveAsync();
        }
    }
}

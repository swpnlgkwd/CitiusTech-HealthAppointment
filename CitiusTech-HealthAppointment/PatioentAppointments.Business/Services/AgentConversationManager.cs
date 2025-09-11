using CitiusTech_HealthAppointmentApis.Dto;
using Microsoft.AspNetCore.Http;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Core.Contracts;
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
        private IUnitOfWork _uow;

        public AgentConversationManager(IAgentConversationsRepository agentConversationsRepository, IHttpContextAccessor httpContextAccessor,
            IUnitOfWork uow)
        {
            this.agentConversationsRepository = agentConversationsRepository;
            this._httpContextAccessor = httpContextAccessor;
            this._uow = uow;
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
            var loggedInUser = await GetLoggedInUserInfo();
            var user = _httpContextAccessor.HttpContext?.User;

            var agentConversation = (await agentConversationsRepository.GetAllAsync())
                .FirstOrDefault(x => x.user_id == loggedInUser.userId.ToString());

            // May return null if the conversation does not exist
            return agentConversation;
        }

        public async Task<string?> FetchThreadIdForLoggedInUser(int usrId)
        {
            string userId = "";

            if (usrId > 0)
            {
                userId = usrId.ToString();
            }

            var agentConversation = (await agentConversationsRepository.GetAllAsync())
                .FirstOrDefault(x => x.user_id == userId);

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


        public async Task<UserInfoDto> GetLoggedInUserInfo()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                throw new Exception("User cannot be null");
            else
            {
                var role = user?.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
                string userId = "";
                var fullName = "";
                if (role == "Provider")
                {
                    userId = await GetProviderId(user);
                }
                else
                {
                    userId = await GetPatientId(user);
                }
                return new UserInfoDto
                {
                    userFullName = fullName,
                    userRole = role,
                    userId = Int32.Parse(userId)
                };
            }
        }

        public async Task<string> GetPatientId(ClaimsPrincipal user)
        {
            var providerIdClaim = user.FindFirst("PatientId")?.Value;
            if (providerIdClaim == null) return "your schedule is ready.";

            int patientId = int.Parse(providerIdClaim);

            var patient = await _uow.Patients.GetByIdAsync(patientId);

            if (patient == null)
                return "User";

            return $"{patient.PatientId}";

        }

        public async Task<string> GetProviderId(ClaimsPrincipal user)
        {
            var providerIdClaim = user.FindFirst("Providerid")?.Value;
            if (providerIdClaim == null) return "your schedule is ready.";

            int providerId = int.Parse(providerIdClaim);

            var provider = await _uow.Patients.GetByIdAsync(providerId);

            if (provider == null)
                return "User";

            return $"{provider.PatientId}";

        }

    }
}

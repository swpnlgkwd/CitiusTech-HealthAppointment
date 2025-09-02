using PatientAppointments.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Contracts
{
    public interface IGreetingManager
    {
        public Task<string> GetGreetingAsync();

        public Task<AgentSummaryResponseDto?> GetDailySummaryAsync();

        public Task<string> GetProviderName(ClaimsPrincipal user);

        public Task<string> GetPatientName(ClaimsPrincipal user);
    }
}

using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using PatientAppointments.Business.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PatientAppointments.Business.Contracts.Risk;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    public class SmartGreetingToolHandler : BaseToolHandler
    {
        private readonly IAppointmentManager _appointmentManager;
        private readonly IAuthManager _authManager;
        private readonly IProviderManager _providerManager;
        private readonly IPatientRiskManager _patientRiskFactorManager;

        public SmartGreetingToolHandler(
            ILogger<SmartGreetingToolHandler> logger,
            IAppointmentManager appointmentManager,
            IAuthManager authManager,
            IProviderManager providerManager,
            IPatientRiskManager patientRiskFactorManager
        ) : base(logger)
        {
            _appointmentManager = appointmentManager;
            _authManager = authManager;
            _providerManager = providerManager;
            _patientRiskFactorManager = patientRiskFactorManager;
        }

        public override string ToolName => SmartGreetingTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            // 🧠 Fetch user context (e.g., name, role)
            var userContext = await _authManager.GetLoggedInUserInfo();
            if (userContext == null)
            {
                _logger.LogWarning("User context not found.");
                return CreateError(call.Id, "User not found.");
            }

            IEnumerable<AppointmentDto>? appointments = null;
            var doctorName = "";

            bool hasRisks = false;
            object? riskDetails = null;

            if (userContext.userRole == "Patient")
            {
                appointments = await _appointmentManager.GetByPatientAsync(userContext.userId);

                // Fetch patient risks — active ones only
                var patientRisks = await _patientRiskFactorManager.GetPatientWithRisksAsync(userContext.userId);

                hasRisks = patientRisks != null;

                // Optional: add more detailed info if needed
                riskDetails = patientRisks;
            }
            else if (userContext.userRole == "Provider")
            {
                appointments = await _appointmentManager.GetByDoctorAsync(userContext.userId);
            }

            var firstAppointment = appointments?.FirstOrDefault();
            doctorName = firstAppointment != null ? (await _providerManager.GetByIdAsync(firstAppointment.ProviderId))?.FullName ?? "" : "";

            var result = new
            {
                userRole = userContext.userRole,
                userName = userContext.userFullName,
                currentTime = DateTime.UtcNow,
                appointmentToday = firstAppointment != null,
                doctorName = doctorName,
                hasRisks = hasRisks,
                risks = riskDetails
            };

            return CreateSuccess(call.Id, "Greeting context fetched successfully.", result);
        }
    }
}

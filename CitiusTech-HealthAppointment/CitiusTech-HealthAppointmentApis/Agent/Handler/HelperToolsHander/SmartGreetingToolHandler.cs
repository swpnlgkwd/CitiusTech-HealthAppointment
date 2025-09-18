using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using PatientAppointments.Business.Dtos;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    public class SmartGreetingToolHandler : BaseToolHandler
    {
        private readonly IAppointmentManager _appointmentManager;
        private readonly IAuthManager _authManager;
        private readonly IProviderManager _providerManager;

        public SmartGreetingToolHandler(
            ILogger<SmartGreetingToolHandler> logger,
            IAppointmentManager appointmentManager,
            IAuthManager authManager,
            IProviderManager providerManager
        ) : base(logger)
        {
            _appointmentManager = appointmentManager;
            _authManager = authManager;
            _providerManager = providerManager;
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

            // 📅 Get today's appointments (could be more than one)
            IEnumerable<AppointmentDto>? appointments = null;
            var doctorName = "";

            if (userContext.userRole == "Patient")
            {
                appointments = await _appointmentManager.GetByPatientAsync(userContext.userId);
                
            }
            else if (userContext.userRole == "Provider")
            {
                appointments = await _appointmentManager.GetByDoctorAsync(userContext.userId);
                
            }

            // ✅ Get first appointment (if any)
            var firstAppointment = appointments?.FirstOrDefault();
            doctorName = firstAppointment != null ? (await _providerManager.GetByIdAsync(firstAppointment.ProviderId))?.FullName : "";



            // 📦 Build response payload
            var result = new
            {
                userRole = userContext.userRole,
                userName = userContext.userFullName,
                currentTime = DateTime.UtcNow, // or use time zone service if needed
                appointmentToday = firstAppointment != null,
                doctorName = doctorName,
            };

            return CreateSuccess(call.Id, "Greeting context fetched successfully.", result);

        }
    }
}

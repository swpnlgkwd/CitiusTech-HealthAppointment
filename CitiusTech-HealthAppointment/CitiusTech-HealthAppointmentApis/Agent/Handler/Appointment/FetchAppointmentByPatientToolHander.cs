using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Services;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class FetchAppointmentByPatientToolHander : BaseToolHandler
    {
        private readonly IAppointmentManager _appointmentsManager;
        public FetchAppointmentByPatientToolHander(
            ILogger<FetchAppointmentByPatientToolHander> logger,
            IAppointmentManager appointmentManager) : base(logger)
        {
            _appointmentsManager = appointmentManager;
        }

        public override string ToolName => FetchAppointmentByPatientTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            {
                int? patientId = root.FetchInt("patientId");
                string? appointmentStatusId = root.TryGetProperty("appointmentStatusId", out var statusProp) ? statusProp.GetString() : null;
                string? startDate = root.TryGetProperty("startDate", out var startProp) ? startProp.GetString() : null;
                string? endDate = root.TryGetProperty("endDate", out var endProp) ? endProp.GetString() : null;

                if (patientId == null)
                {
                    _logger.LogWarning("patientId input is missing");
                    return CreateError(call.Id, "patientId is required.");
                }

                var appointments = await _appointmentsManager.GetByPatientAsync((int)patientId);

                if (appointments == null || !appointments.Any())
                {
                    _logger.LogWarning($"No appointments found for patientId: {patientId}");
                    return CreateError(call.Id, $"No appointments found for patient {patientId}.");
                }

                _logger.LogInformation($"Fetched {appointments.Count()} appointments for patientId: {patientId}");

                var response = new
                {
                    success = true,
                    message = "😊 Appointments fetched successfully!",
                    data = appointments
                };

                return CreateSuccess(call.Id, $"✅ Appointments fetched successfully for patient {patientId}.",
                new { appointments });
            }
            catch
            {
                _logger.LogInformation("Failed fetching the appointments");
                throw new Exception("Unable to retrieve the appointments");
            }
        }
    }
}

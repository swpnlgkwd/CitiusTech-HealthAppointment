using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Dtos;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class SubmitAppointmentToolHandler : BaseToolHandler
    {
        private readonly IAppointmentManager _appointmentManager;

        public SubmitAppointmentToolHandler(ILogger<SubmitAppointmentToolHandler> logger, IAppointmentManager appointmentManager) : base(logger)
        {
            _appointmentManager = appointmentManager;
        }

        public override string ToolName => SubmitAppointmentTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            // Extract parameters from the call
            var patientId = root.GetProperty("patientId").GetInt32();
            var providerId = root.GetProperty("providerId").GetInt32();
            var startDate = root.GetProperty("startDate").GetString();
            var endDate = root.GetProperty("endDate").GetString();
            var providerSlot = root.GetProperty("providerSlot").GetInt32();
            var appointmentType = root.GetProperty("appointmentType").GetInt32();
            var statusId = root.GetProperty("statusId").GetInt32();

            try
            {
                await _appointmentManager.CreateAsync(new AppointmentDto
                {
                    PatientId = patientId,
                    ProviderId = providerId,
                    StartUtc = DateTime.Parse(startDate ?? string.Empty),
                    EndUtc = DateTime.Parse(endDate ?? string.Empty),
                    SlotId = providerSlot,
                    TypeId = appointmentType,
                    StatusId = statusId
                });
                _logger.LogInformation("Appointment submitted successfully in {ToolName} for call {CallId}", ToolName, call.Id);
                return CreateSuccess(call.Id, "Appointment submitted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {ToolName} for call {CallId}", ToolName, call.Id);
                return CreateError(call.Id, "Failed to submit appointment. " + ex.Message);
            }
        }
    }
}

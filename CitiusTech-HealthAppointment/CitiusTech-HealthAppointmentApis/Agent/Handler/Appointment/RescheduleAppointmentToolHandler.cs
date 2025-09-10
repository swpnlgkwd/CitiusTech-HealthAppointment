
using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Entities;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class RescheduleAppointmentToolHandler : BaseToolHandler
    {
        private readonly IAppointmentManager _appointmentManager;
        public RescheduleAppointmentToolHandler(ILogger<RescheduleAppointmentToolHandler> logger, IAppointmentManager appointmentManager) : base(logger)
        {
            _appointmentManager = appointmentManager;
        }

        public override string ToolName => RescheduleAppointmentTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            {
                // Extract parameters from the call
                var patientId = root.GetProperty("patientId").GetInt32();
                var appointmentId = root.GetProperty("appointmentId").GetInt32();
                var newStartDate = root.GetProperty("newStartDate").GetString();
                var newEndDate = root.GetProperty("newEndDate").GetString();
                var newProviderSlot = root.GetProperty("newProviderSlot").GetInt32();
                var newProviderSlotStartTime = root.GetProperty("newProviderSlotStartTime").GetString();
                var newProviderSlotEndTime = root.GetProperty("newProviderSlotEndTime").GetString();

                var existingAppointment = await _appointmentManager.GetByIdAsync(patientId);

                await _appointmentManager.UpdateAsync(new AppointmentDto
                {
                    PatientId = patientId,
                    AppointmentId = existingAppointment.AppointmentId,
                    Notes = "resceduled",
                    ProviderId = existingAppointment.ProviderId,
                    StartUtc = DateTime.Parse($"{newStartDate} {newProviderSlotStartTime}"),
                    EndUtc = DateTime.Parse($"{newEndDate} {newProviderSlotEndTime}"),
                    SlotId = newProviderSlot,
                    TypeId = existingAppointment.TypeId,
                    StatusId = 2 //  '2' indicates a rescheduled status
                });

                _logger.LogInformation("Appointment rescheduled successfully in RescheduleAppointmentToolHandler for call {CallId}", call.Id);
                return CreateSuccess(call.Id, "✅ Appointment rescheduled successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RescheduleAppointmentToolHandler.");
                return CreateError(call.Id, "❌ Failed to reschedule appointment. " + ex.Message);
            }
        }
    }
}

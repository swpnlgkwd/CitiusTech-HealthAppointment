using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Core.Contracts.Repositories;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class CancelAppointmentToolHandler : BaseToolHandler
    {
        private readonly IAppointmentManager _appointmentManager;

        public CancelAppointmentToolHandler(
            ILogger<CancelAppointmentToolHandler> logger,
            IAppointmentManager appointmentManager
        ) : base(logger)
        {
            _appointmentManager = appointmentManager;
        }

        public override string ToolName => CancelAppointmentTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            int? appointmentId = root.FetchInt("appointmentId");
            string reason = root.FetchString("reason") ?? "No reason provided";

            if (appointmentId == null)
            {
                _logger.LogWarning("AppointmentId input is missing");
                return CreateError(call.Id, "AppointmentId input is required.");
            }

            bool cancelled = await _appointmentManager.CancelAsync(appointmentId ?? 0, reason);

            if (!cancelled)
            {
                _logger.LogWarning($"Failed to cancel appointment: {appointmentId}");
                return CreateError(call.Id, $"Failed to cancel appointment {appointmentId}. It may not exist or is already cancelled.");
            }

            _logger.LogInformation($"Cancelled appointment: {appointmentId} with reason: {reason}");

            return CreateSuccess(
                call.Id,
                $"✅ Appointment {appointmentId} cancelled successfully.",
                new { appointmentId, reason }
            );
        }
    }

}

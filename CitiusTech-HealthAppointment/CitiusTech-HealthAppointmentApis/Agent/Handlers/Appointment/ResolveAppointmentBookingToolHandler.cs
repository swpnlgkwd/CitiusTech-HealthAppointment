using System.Text.Json;
using Azure.AI.Agents;
using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Common.Handlers;
using CitiusTech_HealthAppointmentApis.Common.Exceptions;
using CitiusTech_HealthAppointmentApis.Common.Extensions;
using CitiusTech_HealthAppointmentApis.Services.Interfaces;
using CitiusTech_HealthAppointmentApis.Agent.Tools.AppointmentBooking;

namespace CitiusTech_HealthAppointmentApis.Agent.Handlers.AppointmentBooking
{
    public class ResolveAppointmentBookingToolHandler : BaseToolHandler
    {
        private readonly IAppointmentService _appointmentService;
        public ResolveAppointmentBookingToolHandler(
            IAppointmentService appointmentService,
            ILogger<ResolveAppointmentBookingToolHandler> logger)
            : base(logger)
        {
            _appointmentService = appointmentService;
        }

        public override string ToolName => ResolveAppointmentBookingTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            {
                // ...existing code...
                int? patientIdNullable = root.FetchInt("patientId");
                int? providerIdNullable = root.FetchInt("providerId");
                int? slotIdNullable = root.FetchInt("slotId");
                int? statusIdNullable = root.FetchInt("statusId");
                int? typeIdNullable = root.FetchInt("typeId");
                string? notes = root.FetchString("notes");

                if (!patientIdNullable.HasValue || !providerIdNullable.HasValue || !slotIdNullable.HasValue || !statusIdNullable.HasValue || !typeIdNullable.HasValue)
                    return CreateError(call.Id, "❌ Invalid input parameters.");

                int patientId = patientIdNullable.Value;
                int providerId = providerIdNullable.Value;
                int slotId = slotIdNullable.Value;
                int statusId = statusIdNullable.Value;
                int typeId = typeIdNullable.Value;
                
                // ...existing code... ̰
                if (patientId <= 0 || providerId <= 0 || slotId <= 0 || statusId <= 0 || typeId <= 0)
                    return CreateError(call.Id, "❌ Invalid input parameters.");

                var result = await _appointmentService.AppointmentBookingAsync(patientId, providerId, slotId, statusId, typeId, notes);

                if (result == null)
                    return CreateError(call.Id, "⚠️ Unable to book appointment. Please check slot availability.");

                return CreateSuccess(call.Id, "✅ Appointment booked successfully.", result);
            }
            catch (BusinessRuleException brex)
            {
                return CreateError(call.Id, $"⚠️ {brex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AppointmentBooking: Unexpected error occurred.");
                return CreateError(call.Id, "❌ An internal error occurred while booking appointment.");
            }
        }
    }
}
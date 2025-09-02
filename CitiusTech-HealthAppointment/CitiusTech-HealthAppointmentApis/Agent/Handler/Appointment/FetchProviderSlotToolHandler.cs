
using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Core.Entities;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class FetchProviderSlotToolHandler : BaseToolHandler
    {
        private readonly IAppointmentManager _appointmentManager;
        public FetchProviderSlotToolHandler(
            ILogger<FetchProviderSlotToolHandler> logger,
            IAppointmentManager appointmentManager  ) : base(logger)
        {
            _appointmentManager = appointmentManager;
        }

        public override string ToolName => FetchProviderSlotTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            // 🔍 Parse the filters
            int? providerId = root.FetchInt("providerId");
            DateTime? sDate = root.FetchDateTime("startDate");
            DateTime? eDate = root.FetchDateTime("endDate");

            if (providerId == null)
            {
                throw new Exception("Provider Id could not be null");
            }
            else if( sDate == null)
            {
                throw new Exception("Schedule date could not be null");
            }
            else
            {
                try
                {
                    _logger.LogInformation("Fetching the provider slots for given provider");
                    var providerSlots = await _appointmentManager.GetProviderSlotsAsync(providerId ?? 1, sDate ?? DateTime.Now, eDate);
                    return CreateSuccess(call.Id, "Provider slots fetched successfully", providerSlots);
                }
                catch
                {
                    _logger.LogInformation("Failed fetching the Provider slots");
                    throw new Exception("Unable to retrieve the Provider slots");
                }
            }
        }
    }
}

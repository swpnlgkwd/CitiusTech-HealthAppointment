using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class FetchAppointmentTypeToolHandler : BaseToolHandler
    {

        private readonly IAppointmentManager _appointmentManager;
        public FetchAppointmentTypeToolHandler(
            IAppointmentManager appointmentManager,
            ILogger<FetchAppointmentTypeToolHandler> logger) : base(logger)
        {
            _appointmentManager = appointmentManager;
        }

        public override string ToolName => FetchAppointmentTypeTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            {
                _logger.LogInformation("Fetching the appointment types");
                var appTypes = await _appointmentManager.GetAppointmentTypesAsync();
                return CreateSuccess(call.Id, "Appointment types fetched successfully", appTypes.ToList());
            }
            catch
            {
                _logger.LogInformation("Failed fetching the appointment types");
                throw new Exception("Unable to retrieve the appointment type");
            }
        }
    }
}

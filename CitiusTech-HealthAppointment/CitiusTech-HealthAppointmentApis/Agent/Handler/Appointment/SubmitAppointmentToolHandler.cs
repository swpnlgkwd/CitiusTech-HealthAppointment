using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class SubmitAppointmentToolHandler : BaseToolHandler
    {
        public SubmitAppointmentToolHandler(ILogger<SubmitAppointmentToolHandler> logger) : base(logger)
        {
        }

        public override string ToolName => SubmitAppointmentTool.GetTool().Name;

        public override Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            throw new NotImplementedException();
        }
    }
}

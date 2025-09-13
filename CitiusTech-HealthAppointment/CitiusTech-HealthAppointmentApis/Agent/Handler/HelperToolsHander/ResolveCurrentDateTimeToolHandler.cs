using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    public class ResolveCurrentDateTimeToolHandler : BaseToolHandler
    {
        public ResolveCurrentDateTimeToolHandler(
            ILogger<ResolveCurrentDateTimeToolHandler> logger
        ) : base(logger)
        {
        }

        public override string ToolName => ResolveCurrentDateTimeTool.GetTool().Name;

        public override Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            var currentDateTime = DateTime.UtcNow; // Use UtcNow for consistency
            var localDateTime = DateTime.Now;      // Local system time

            _logger.LogInformation($"Fetched current date/time: {localDateTime} (local), {currentDateTime} (UTC)");

            return Task.FromResult<ToolOutput?>(CreateSuccess(
                call.Id,
                "✅ Current date and time fetched successfully.",
                new
                {
                    utcDateTime = currentDateTime.ToString("o"),   // ISO 8601 format
                    localDateTime = localDateTime.ToString("f"),  // Human-readable format
                    timezone = TimeZoneInfo.Local.StandardName
                }
            ));
        }
    }

}

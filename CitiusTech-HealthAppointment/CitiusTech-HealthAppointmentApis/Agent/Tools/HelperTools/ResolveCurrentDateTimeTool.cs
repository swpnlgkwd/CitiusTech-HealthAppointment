using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools
{
    public class ResolveCurrentDateTimeTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "resolveCurrentDateTime",
                description:
                    "Use this tool to fetch the current system date and time. " +
                    "This is helpful for validating appointment schedules, showing the current date, or logging events.",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new { }, // No input needed
                        required = Array.Empty<string>()
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }

}

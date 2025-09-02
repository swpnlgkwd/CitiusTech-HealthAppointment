using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class FetchProviderSlotTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "fetchProviderSlot",
                description: "Use this tool to retrieve available provider slots for specific provider Id with optional filters like start date, end date, booking status",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            providerId = new
                            {
                                type = "integer",
                                description = "Required. Filter by the provider id."
                            },
                            startDate = new
                            {
                                type = "string",
                                format = "date",
                                description = "Optional. Start date of the slot filter (yyyy-MM-dd)."
                            },
                            endDate = new
                            {
                                type = "string",
                                format = "date",
                                description = "Optional. End date of the slot filter (yyyy-MM-dd)."
                            },
                            bookingStatus = new
                            {
                                type = "boolean",
                                description = "Optional. booking status filter in true or false"
                            }
                        }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }
}

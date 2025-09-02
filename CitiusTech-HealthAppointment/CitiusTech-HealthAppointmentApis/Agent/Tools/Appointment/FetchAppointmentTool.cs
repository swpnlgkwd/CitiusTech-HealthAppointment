using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class FetchAppointmentTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "fetchAppointment",
                description: "Use this tool to retrieve appointments by user Id with optional filters like appointment type, specific date or date range",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            userId = new
                            {
                                type = "integer",
                                description = "Required. Filter by the patient id or doctor id based on role."
                            },                           
                            appointmentStatus = new
                            {
                                type = "integer",
                                description = "Optional. Filter by appointment status: Booked, Rescheduled, Cancelled, Completed or NoShow",
                            },
                            startDate = new
                            {
                                type = "string",
                                format = "date",
                                description = "Optional. Start date of the appointment filter (yyyy-MM-dd)."
                            },
                            endDate = new
                            {
                                type = "string",
                                format = "date",
                                description = "Optional. End date of the appointment filter (yyyy-MM-dd)."
                            }
                        }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }
}

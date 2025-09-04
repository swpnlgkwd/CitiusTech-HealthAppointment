using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class FetchAppointmentByDoctorTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "fetchAppointmentByDoctor",
                description: "Use this tool to retrieve appointments by doctor with optional filters like appointment type, specific date or date range",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            providerId = new
                            {
                                type = "integer",
                                description = "Required. Filter by the provider/doctor id."
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
                        },
                        required = new[] { "providerId" }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }
}

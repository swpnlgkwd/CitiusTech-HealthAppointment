using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class FetchAppointmentByPatientTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "fetchAppointmentByPatient",
                description: "Use this tool to when user says Show my appointments or show my appointment for tomorrow",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            patientId = new
                            {
                                type = "integer",
                                description = "Required. Filter by the patient id."
                            },                           
                            appointmentStatusId = new
                            {
                                type = "integer",
                                description = "The ID of the appoinment status.",
                            },
                            startDate = new
                            {
                                type = "string",
                                format = "date",
                                description = "Start date if provided otherwise current date (yyyy-MM-dd)."
                            },
                            endDate = new
                            {
                                type = "string",
                                format = "date",
                                description = "End date If provided otherwise null (yyyy-MM-dd)."
                            }
                        },
                        required = new[] { "patientId" }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }
}

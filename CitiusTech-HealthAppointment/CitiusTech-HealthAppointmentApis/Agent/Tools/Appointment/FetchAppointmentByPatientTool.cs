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
                description:
                    "Use this tool to fetch appointments for a given patient. " +
                    "You must provide a patientId. Optionally, you can filter by appointmentStatusId, startDate, or endDate.",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            patientId = new
                            {
                                type = "string",
                                description = "The unique patient ID. Example: '213'."
                            },
                            appointmentStatusId = new
                            {
                                type = "string",
                                description = "Optional filter: Appointment status ID. Example: '1' (Booked), '2' (Rescheduled), '3' (Cancelled)."
                            },
                            startDate = new
                            {
                                type = "string",
                                description = "Optional filter: Start date for appointments (ISO 8601 format). Example: '2025-09-01'."
                            },
                            endDate = new
                            {
                                type = "string",
                                description = "Optional filter: End date for appointments (ISO 8601 format). Example: '2025-09-30'."
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

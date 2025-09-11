using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class CancelAppointmentTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "cancelAppointment",
                description:
                    "Use this tool to cancel a scheduled appointment using its unique appointment ID. " +
                    "⚠️ Only use this tool when the appointmentId is already known (from previous queries or listings).",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            appointmentId = new
                            {
                                type = "integer",
                                description = "The unique appointment ID to be cancelled. Example: '565'."
                            },
                            reason = new
                            {
                                type = "string",
                                description = "The reason for cancelling the appointment (optional). Example: 'Patient request', 'Doctor unavailable'."
                            }
                        },
                        required = new[] { "appointmentId" }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }

}

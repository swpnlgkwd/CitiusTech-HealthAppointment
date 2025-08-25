using Azure.AI.Agents;
using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.AppointmentBooking
{
    public static class ResolveAppointmentBookingTool
    {
        public static FunctionToolDefinition GetTool()
        {
            var parameters = BinaryData.FromObjectAsJson(
                new
                {
                    type = "object",
                    properties = new
                    {
                        patientId = new
                        {
                            type = "integer",
                            description = "Patient ID"
                        },
                        providerId = new
                        {
                            type = "integer",
                            description = "Provider ID"
                        },
                        slotId = new
                        {
                            type = "integer",
                            description = "Slot ID"
                        },
                        statusId = new
                        {
                            type = "integer",
                            description = "Status ID"
                        },
                        typeId = new
                        {
                            type = "integer",
                            description = "Type ID"
                        },
                        notes = new
                        {
                            type = "string",
                            description = "Notes"
                        }
                    },
                    required = new[] { "patientId", "providerId", "slotId", "statusId", "typeId" }
                },
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            // Use constructor instead of object initializer
            return new FunctionToolDefinition(
                name: "bookAppointment",
                description: "Books an appointment for a patient with a provider.",
                parameters: parameters
            );
        }
    }
}
using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class RescheduleAppointmentTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "rescheduleAppointment",
                description: "Use this tool when a user want to rescedule the apointment",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            patientId = new { type = "integer", description = "The ID or userId of the patient (logged In user) applying for appointment." },
                            appointmentId = new { type = "integer", description = "The ID of the existing appointment." },
                            newStartDate = new { type = "string", format = "date", description = "Appointment start date in yyyy-mm-dd format." },
                            newEndDate = new { type = "string", format = "date", description = "Appointment end date in yyyy-MM-dd format." },
                            newProviderSlot = new { type = "integer", description = " The ID of the provider slot." },
                            newProviderSlotStartTime = new { type = "string", format = "time", description = "The provider slot start time in 24-hour HH:mm format." },
                            newProviderSlotEndTime = new { type = "string", format = "time", description = "The provider slot end time in 24-hour HH:mm format." },
                        },
                        required = new[] { "patientId", "appointmentId", "newStartDate", "newEndDate", "newProviderSlot", "newProviderSlotStartTime", "newProviderSlotEndTime" }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }
}

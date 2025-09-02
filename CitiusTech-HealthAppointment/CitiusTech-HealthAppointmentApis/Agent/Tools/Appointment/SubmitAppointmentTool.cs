using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class SubmitAppointmentTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "submitAppointment",
                description: "Use this tool when a user says book an appointment, It will create new appointment for that user with start date, end date, patient Id, provider Id, provider slot Id, appointment type, initial note etc., The new appointment is submitted with status 'Booked' by default.",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            patientId = new { type = "integer", description = "The ID of the patient applying for appointment." },
                            providerId = new { type = "integer", description = "The ID of the provider." },
                            startDate = new { type = "string", format = "date", description = "Appointment start date in yyyy-mm-dd format" },
                            endDate = new { type = "string", format = "date", description = "Appointment end date in yyyy-MM-dd format." },
                            providerSlot = new { type = "integer", description = " The ID of the provider slot" },
                            appointmentType = new { type = "string", description = "Type of Appointment (e.g., Consultation, Follow-up, Telehealth, Emergency)." }
                        },
                        required = new[] { "patientId", "providerId", "startDate", "endDate", "providerSlot", "appointmentType" }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }
}

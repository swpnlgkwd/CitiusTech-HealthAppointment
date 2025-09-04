using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class FetchDoctorInfoBySpecialtyTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "fetchDoctorInfoBySpecialty",
                description:
                    "Fetches doctor information (name, specialization, availability, etc.) for a given medical specialty ID. " +
                "Use this tool after resolving the patient’s symptoms into a medical specialty with 'resolveSpeciality'. " +
                "Do not pass the speciality name directly — always use the corresponding specialityId.",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            specialityId = new
                            {
                                type = "string",
                                description =
                                    "The unique ID of the resolved medical specialty. " +
                                    "This ID is mapped internally in the database (e.g., Cardiology=101, Dermatology=102, etc.)."
                            }
                        },
                        required = new[] { "specialityId" }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }

}

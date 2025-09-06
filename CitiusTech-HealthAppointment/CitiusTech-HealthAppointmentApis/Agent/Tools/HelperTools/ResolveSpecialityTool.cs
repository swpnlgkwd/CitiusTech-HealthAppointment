using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools
{
    public class ResolveSpecialityTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "resolveSpeciality",
                description:
                    "Use this tool **only after you have analyzed the patient’s symptoms and resolved them into a medical specialty**. " +
                    "Do not pass raw symptoms into this tool. Instead, provide the resolved specialty name as the input. " +
                    "The available specialties are strictly limited to: 'Cardiology', 'Dermatology', 'Pediatrics', or 'General Practitioner'. " +
                    "Examples: 'chest pain' → Cardiology, 'skin rash' → Dermatology, 'child fever' → Pediatrics. " +
                    "If no clear match is found, always default to 'General Practitioner'.",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            speciality = new
                            {
                                type = "string",
                                description =
                                    "The resolved medical specialty name. Must be one of: 'Cardiology', 'Dermatology', 'Pediatrics', 'General Practitioner'. " +
                                    "Example: 'Cardiology', 'Dermatology', 'Pediatrics', 'General Practitioner'."
                            }
                        },
                        required = new[] { "speciality" }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }



    }
}

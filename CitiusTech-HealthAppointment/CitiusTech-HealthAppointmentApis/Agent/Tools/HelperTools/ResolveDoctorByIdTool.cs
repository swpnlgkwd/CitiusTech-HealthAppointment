using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools
{
    public class ResolveDoctorByIdTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "resolveDoctorById",
                description:
                    "Fetch detailed information about a doctor using their unique provider ID. " +
                    "Use this tool only when the doctor’s ID is already known (e.g., from a previous list of doctors).",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            providerId = new
                            {
                                type = "string",
                                description = "The unique provider/doctor ID. Example: 'D001', 'D002'."
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

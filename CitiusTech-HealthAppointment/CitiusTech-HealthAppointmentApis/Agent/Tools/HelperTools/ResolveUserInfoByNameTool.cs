using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools
{
    public class ResolveUserInfoByNameTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "resolveStaffInfoByName",
                description: "Finds one or more active user name based on a full or partial name (e.g., 'Chris', 'Anderson', 'Lee'). "
                           + "Returns matching user details such as user ID, name, role",
                parameters: BinaryData.FromObjectAsJson(
                    new
                    {
                        type = "object",
                        properties = new
                        {
                            name = new
                            {
                                type = "string",
                                description = "Full or partial staff name (e.g., 'Asha', 'Patil', 'Sunita')"
                            }
                        },
                        required = new[] { "name" }
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                )
            );
        }
    }
}

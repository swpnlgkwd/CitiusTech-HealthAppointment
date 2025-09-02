using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace HospitalSchedulingApp.Agent.Tools.HelperTools
{

    public static class ResolveLoggedInUserInfoTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
               name: "resolveLoggedInUserInfo",
               description: "Resolves the login user and its role (e.g., Employee or Scheduler). Useful for determining access permissions.",
               parameters: BinaryData.FromObjectAsJson(
                   new
                   {
                       type = "object",
                       properties = new { }, // No parameters
                   },
                   new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
               )
           );
        }
    }

}

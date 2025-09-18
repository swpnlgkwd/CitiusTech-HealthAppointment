using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools
{
    public class SmartGreetingTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "SmartGreeting",
                description:
                    "Use this tool to fetch personalized greeting context for a patient or hospital staff. " +
                    "It returns user info, appointment status, and any clinical risk indicators. " +
                    "This is typically used to generate a context-aware greeting message in the assistant dashboard."
                
            );
        }
    }
}

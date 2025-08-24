using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.AppointmentBooking;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools
{
    public static class FunctionToolDefinitions
    {
        public static IReadOnlyList<FunctionToolDefinition> All => new[]
        {
            
            ResolveAppointmentBookingTool.GetTool()
        };
    }
}

using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class FetchAppointmentTypeTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
               name: "fetchAppointmentType",
               description: "Use this tool to find the appointment type when user says book an appointemnt or retrive/fetch my appointment(s)"
            );
        }
    }
}

using Azure.AI.Agents.Persistent;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment
{
    public class FetchDoctorInfoTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "fetchDoctorInfo",
                description:
                    "Use this tool to fetch detailed information about all doctors"                
            );
        }
    }

}

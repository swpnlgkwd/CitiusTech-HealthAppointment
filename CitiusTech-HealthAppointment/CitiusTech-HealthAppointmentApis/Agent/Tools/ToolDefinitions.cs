using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using HospitalSchedulingApp.Agent.Tools.HelperTools;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools
{
    public static class ToolDefinitions
    {
        public static IReadOnlyList<FunctionToolDefinition> All => new[]
        {
            ResolveNaturalLanguageDateTool.GetTool(),
            ResolveDoctorInfoByNameTool.GetTool(),
            ResolveDoctorSpecialityTool.GetTool(),
            ResolveRelativeDateTool.GetTool(),
            ResolveLoggedInUserInfoTool.GetTool(),
            ResolveUserInfoByNameTool.GetTool(),
            FetchProviderSlotTool.GetTool(),
            FetchAppointmentByPatientTool.GetTool(),
            FetchAppointmentTypeTool.GetTool(),
            SubmitAppointmentTool.GetTool(),
            FetchAppointmentByDoctorTool.GetTool(),
            FetchAppointmentByPatientTool.GetTool()
        };
    }
}

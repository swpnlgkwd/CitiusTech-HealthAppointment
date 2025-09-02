using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Common;
using HospitalSchedulingApp.Agent.Tools.HelperTools;
using PatientAppointments.Business.Contracts;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    public class ResolveLoggedInUserRoleToolHandler : BaseToolHandler
    {
        private readonly IAuthManager _authManager;
        public ResolveLoggedInUserRoleToolHandler(
            ILogger<ResolveLoggedInUserRoleToolHandler> logger,
            IAuthManager authManager) : base(logger)
        {
            this._authManager = authManager;
        }

        public override string ToolName =>  ResolveLoggedInUserRoleTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            { 
                var result = await _authManager.GetUserRole();

                _logger.LogInformation("Retrieved User role information", result.FirstOrDefault());
                return CreateSuccess(call.Id, "✅ User role resolved successfully.", result.FirstOrDefault());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving relative date.");
                return null;
            }
        }
    }
}

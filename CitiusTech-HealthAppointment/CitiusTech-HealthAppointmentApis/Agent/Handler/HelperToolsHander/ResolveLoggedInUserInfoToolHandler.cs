using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Common;
using HospitalSchedulingApp.Agent.Tools.HelperTools;
using PatientAppointments.Business.Contracts;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    public class ResolveLoggedInUserInfoToolHandler : BaseToolHandler
    {
        private readonly IAuthManager _authManager;
        public ResolveLoggedInUserInfoToolHandler(
            ILogger<ResolveLoggedInUserInfoToolHandler> logger,
            IAuthManager authManager) : base(logger)
        {
            this._authManager = authManager;
        }

        public override string ToolName =>  ResolveLoggedInUserInfoTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            { 
                var result = await _authManager.GetLoggedInUserInfo();

                _logger.LogInformation("Retrieved User information", result);
                return CreateSuccess(call.Id, "✅ User info resolved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving relative date.");
                return null;
            }
        }
    }
}

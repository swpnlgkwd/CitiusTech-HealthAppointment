using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    public class ResolveUserInfoByNameToolHandler: BaseToolHandler
    {
        private readonly IAuthManager _authManager;

        public ResolveUserInfoByNameToolHandler(ILogger<ResolveUserInfoByNameToolHandler> logger, IAuthManager authManager)
            : base(logger) // ✅ common logging/error helpers
        {
            _authManager = authManager;
        }

        public override string ToolName => ResolveDoctorInfoByNameTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            string name = root.FetchString("name") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("name is missing");
                return CreateError(call.Id, "user name is required.");
            }
            var result = await _authManager.GetUsersByName(name);

            _logger.LogInformation("Retrieved user's information", result);
            return CreateSuccess(call.Id, "✅ User resolved successfully.", result);
        }
    }
}

using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Core.Entities;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    /// <summary>
    /// Handler to resolve natural language date(s) into a standardized date range (yyyy-MM-dd).
    /// Supports single date or a range in any order, with or without years.
    /// </summary>
    public class ResolveDoctorInfoByNameToolHandler : BaseToolHandler
    {
        // Inject
       // private readonly IDoctorInfoManager _doctorInfoManager;
        public ResolveDoctorInfoByNameToolHandler(ILogger<ResolveNaturalLanguageDateToolHandler> logger)
            : base(logger) // ✅ common logging/error helpers
        {
        }

        public override string ToolName => ResolveDoctorInfoByNameTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            string name = root.FetchString("name") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogWarning("name is missing");
                return CreateError(call.Id, "Date input is required.");
            }
            //var result = await _doctorInfoManager.FetchDoctorInfoByNamePart(name);
            var result =  new Provider
            {
                ProviderId = 123,

            };
            _logger.LogInformation("Retrieved doctor information", result);
            return CreateSuccess(call.Id, "✅ Date(s) resolved successfully.", result);
        }
    }
}

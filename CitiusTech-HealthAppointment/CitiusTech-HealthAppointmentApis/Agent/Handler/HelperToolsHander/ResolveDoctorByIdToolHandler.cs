using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    public class ResolveDoctorByIdToolHandler : BaseToolHandler
    {
        private readonly IProviderManager _providerManager;

        public ResolveDoctorByIdToolHandler(
            ILogger<ResolveDoctorByIdToolHandler> logger,
            IProviderManager providerManager
        ) : base(logger)
        {
            _providerManager = providerManager;
        }

        public override string ToolName => ResolveDoctorByIdTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            {
                int? providerId = root.FetchInt("providerId");

                if (providerId == null)
                {
                    _logger.LogWarning("ProviderId input is missing");
                    return CreateError(call.Id, "ProviderId input is required.");
                }

                var doctor = await _providerManager.GetByIdAsync((int)providerId);

                if (doctor == null)
                {
                    _logger.LogWarning($"No doctor found for providerId: {providerId}");
                    return CreateError(call.Id, $"No doctor found for providerId: {providerId}.");
                }

                _logger.LogInformation($"Fetched doctor details for providerId: {providerId}");

                return CreateSuccess(
                    call.Id,
                    $"✅ Doctor details fetched successfully.",
                    new { doctor }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving doctor by ID.");
                return CreateError(call.Id, "❌ Failed to resolve doctor by ID. " + ex.Message);
            }
        }
    }

}

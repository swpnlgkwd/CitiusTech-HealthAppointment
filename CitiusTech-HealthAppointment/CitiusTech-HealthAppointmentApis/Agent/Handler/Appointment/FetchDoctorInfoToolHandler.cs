using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Core.Entities;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class FetchDoctorInfoToolHandler : BaseToolHandler
    {
        private readonly IProviderManager _providerManager;

        public FetchDoctorInfoToolHandler(
            ILogger<FetchDoctorInfoToolHandler> logger,
            IProviderManager providerManager
        ) : base(logger)
        {
            _providerManager = providerManager;
        }

        public override string ToolName => FetchDoctorInfoTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            {
                var doctor = await _providerManager.GetAllAsync();

                if (doctor == null)
                {
                    _logger.LogWarning($"No doctors found.");
                    return CreateError(call.Id, $"No doctors found.");
                }

                _logger.LogInformation($"Fetched doctor details.");

                return (CreateSuccess(
                    call.Id,
                    $"✅ Doctor details fetched successfully.",
                    new { doctor }
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching doctor info.");
                return CreateError(call.Id, "❌ Failed to fetch doctor info. " + ex.Message);
            }


        }
    }

}

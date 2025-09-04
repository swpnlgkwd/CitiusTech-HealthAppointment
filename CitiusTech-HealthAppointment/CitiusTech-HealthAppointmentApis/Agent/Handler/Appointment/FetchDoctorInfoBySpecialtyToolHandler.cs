using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Core.Contracts.Repositories;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class FetchDoctorInfoBySpecialtyToolHandler : BaseToolHandler
    {
        private readonly IProviderManager _providerManager;

        public FetchDoctorInfoBySpecialtyToolHandler(
            ILogger<FetchDoctorInfoBySpecialtyToolHandler> logger,
            IProviderManager providerManager
        ) : base(logger)
        {
            _providerManager = providerManager;
        }

        public override string ToolName => FetchDoctorInfoBySpecialtyTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            int? specialityId = root.FetchInt("specialityId") ?? 0;

            if (specialityId == 0)
            {
                _logger.LogWarning("SpecialityId input is missing");
                return CreateError(call.Id, "Speciality Id input is required.");
            }

            // Fetch doctors by speciality from repository / DB
            var doctors = (await _providerManager.GetAllAsync()).Where(x=>x.SpecialtyId == specialityId).ToList();

            if (doctors == null || !doctors.Any())
            {
                _logger.LogWarning($"No doctors found for specialityId: {specialityId}");
                return CreateError(call.Id, $"No doctors found for speciality: {specialityId}.");
            }

            _logger.LogInformation($"Found {doctors.Count()} doctors for specialityId: {specialityId}");

            return CreateSuccess(
                call.Id,
                $"✅ Doctors fetched successfully.",
                new { doctors }
            );
        }
    }

}

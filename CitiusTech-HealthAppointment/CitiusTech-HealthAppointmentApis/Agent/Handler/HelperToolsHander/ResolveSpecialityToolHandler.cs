using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    public class ResolveSpecialityToolHandler : BaseToolHandler
    {
        private readonly ISpecialtyManager _specialtyManager;

        public ResolveSpecialityToolHandler(
            ILogger<ResolveSpecialityToolHandler> logger,
            ISpecialtyManager specialtyManager
            ) : base(logger)
        {
            _specialtyManager = specialtyManager;
        }

        public override string ToolName => ResolveSpecialityTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            string speciality = root.FetchString("speciality") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(speciality))
            {
                _logger.LogWarning("Speciality input is missing");
                return (CreateError(call.Id, "Speciality input is required."));
            }

            // ✅ Optional DB lookup (if needed to map speciality to a department)
            var specialty = (await _specialtyManager.GetAllAsync()).ToList().Where(x => x.SpecialtyName.Contains(speciality)).FirstOrDefault();

            _logger.LogInformation($"Resolved specialty: {specialty}");

            return CreateSuccess(
                call.Id,
                $"✅ Specialty resolved successfully.",
                new
                {
                    speciality = specialty?.SpecialtyName ?? "General Practitioner",
                    specialityId = specialty?.SpecialityId.ToString() ?? "0"
                }
            );
        }
    }
}

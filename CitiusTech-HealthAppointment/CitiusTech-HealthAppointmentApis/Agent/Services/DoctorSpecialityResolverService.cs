using Microsoft.EntityFrameworkCore;
using PatientAppointments.Core.Entities;

namespace CitiusTech_HealthAppointmentApis.Agent.Services
{
    public class DoctorSpecialityResolverService : IDoctorSpecialityResolverService
    {
        private readonly ILogger<DoctorSpecialityResolverService> _logger;
        private readonly IAgentService _agentService;
        private readonly AppDbContext _db;

        public DoctorSpecialityResolverService(
            ILogger<DoctorSpecialityResolverService> logger,
            IAgentService agentService,
            AppDbContext db)
        {
            _logger = logger;
            _agentService = agentService;
            _db = db;
        }

        public async Task<(SpecialityEnum? specialityEnum, string? department)> ResolveDepartmentAsync(string symptoms)
        {
            if (string.IsNullOrWhiteSpace(symptoms))
            {
                _logger.LogWarning("Empty symptoms passed to resolver");
                return (null, null);
            }

            // 1️⃣ Ask the agent for a department suggestion
            var prompt = $"Patient symptoms: '{symptoms}'. " +
                         $"From these departments {string.Join(", ", await _db.Departments.Select(d => d.Name).ToListAsync())}, " +
                         $"suggest the most appropriate one. Return only the department name.";

            var response = await _agentService.GetAgentResponseAsync(MessageRole.User, prompt);

            var suggestedDepartment = (response as MessageTextContent)?.Text?.Trim();

            if (string.IsNullOrWhiteSpace(suggestedDepartment))
            {
                _logger.LogInformation("Agent returned no valid department for symptoms: {Symptoms}", symptoms);
                return (null, null);
            }

            // 2️⃣ Check against DB
            var departmentEntity = await _db.Departments
                .FirstOrDefaultAsync(d => d.Name.Equals(suggestedDepartment, StringComparison.OrdinalIgnoreCase));

            if (departmentEntity == null)
            {
                _logger.LogInformation("Agent suggested department '{Dept}' not found in DB", suggestedDepartment);
                return (null, null);
            }

            // 3️⃣ Map to Enum if possible
            if (!Enum.TryParse(typeof(SpecialityEnum), departmentEntity.Name, true, out var specialityObj))
            {
                _logger.LogWarning("DB department '{Dept}' has no matching SpecialityEnum", departmentEntity.Name);
                return (null, departmentEntity.Name);
            }

            return ((SpecialityEnum)specialityObj, departmentEntity.Name);
        }
    }
}

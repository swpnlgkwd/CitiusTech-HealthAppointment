using Azure.AI.Agents.Persistent;
using Microsoft.EntityFrameworkCore;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;

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

       public async Task<string?> ResolveDepartmentAsync(string symptoms)
       {
           if (string.IsNullOrWhiteSpace(symptoms))
           {
               _logger.LogWarning("Empty symptoms passed to resolver");
               return null;
           }

           // 1️⃣ Ask the agent for a department suggestion
           var prompt = $"Patient symptoms: '{symptoms}'. " +
                        $"From these departments {string.Join(", ", await _db.Specialty.Select(d => d.SpecialtyName).ToListAsync())}, " +
                        $"suggest the most appropriate one. Return only the department Name.";

           var response = await _agentService.GetAgentResponseAsync(MessageRole.User, prompt);

           var suggestedDepartment = (response as MessageTextContent)?.Text?.Trim();

           if (string.IsNullOrWhiteSpace(suggestedDepartment))
           {
               _logger.LogInformation("Agent returned no valid department for symptoms: {Symptoms}", symptoms);
               return null;
           }

           // 2️⃣ Check against DB
           var departmentEntity = await _db.Specialty
               .FirstOrDefaultAsync(d => d.SpecialtyName.Equals(suggestedDepartment, StringComparison.OrdinalIgnoreCase));

           if (departmentEntity == null)
           {
               _logger.LogInformation("Agent suggested department '{Dept}' not found in DB", suggestedDepartment);
               return null;
           }

           // 3️⃣ Map to Enum if possible
           if (!Enum.TryParse(typeof(SpecialityEnum), departmentEntity.SpecialtyName, true, out var specialityObj))
           {
               _logger.LogWarning("DB department '{Dept}' has no matching SpecialityEnum", departmentEntity.SpecialtyName);
               return departmentEntity.SpecialtyName;
           }

           return departmentEntity.SpecialtyName;
       }
    }
}

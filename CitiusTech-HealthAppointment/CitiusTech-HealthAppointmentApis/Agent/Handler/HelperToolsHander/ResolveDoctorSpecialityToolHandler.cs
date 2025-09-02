using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Entities;
using CitiusTech_HealthAppointmentApis.Agent.Services;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    /// <summary>
    /// Tool handler that resolves patient symptoms into a doctor speciality (department).
    /// Maps symptoms to a known SpecialityEnum and returns provider/department details.
    /// </summary>
    public class ResolveDoctorSpecialityToolHandler : BaseToolHandler
    {
        // private readonly Dictionary<string, string> _symptomDepartmentMap = new(StringComparer.OrdinalIgnoreCase)
        //  {
        //      { "chest pain", "Cardiology" },
        //      { "heart", "Cardiology" },
        //      { "skin rash", "Dermatology" },
        //      { "acne", "Dermatology" },
        //      { "fever", "GeneralMedicine" },
        //      { "flu", "GeneralMedicine" },
        //      { "fracture", "Orthopedics" },
        //      { "bone pain", "Orthopedics" },
        //      { "asthma", "Pulmonology" },
        //      { "breathing", "Pulmonology" },
        //      { "pregnancy", "Gynecology" },
        //      { "child", "Pediatrics" }
        //  };

        private readonly IDoctorSpecialityResolverService _resolverService;
 
        public ResolveDoctorSpecialityToolHandler(
            IDoctorSpecialityResolverService resolverService,
            ILogger<ResolveDoctorSpecialityToolHandler> logger)
            : base(logger)
        {
            _resolverService = resolverService;
        }
        /// <summary>
        /// Tool name as defined in the schema.
        /// </summary>
        public override string ToolName => ResolveDoctorSpecialityTool.GetTool().Name;

        /// <summary>
        /// Handles resolving symptoms → department → SpecialityEnum.
        /// </summary>
        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            {
                string symptoms = root.FetchString("symptoms") ?? string.Empty;

                if (string.IsNullOrWhiteSpace(symptoms))
                {
                    _logger.LogWarning("Symptoms input missing in ResolveDoctorSpecialityToolHandler");
                    return CreateError(call.Id, "❌ Symptoms input is required.");
                }

                var matchedDepartment = await _resolverService.ResolveDepartmentAsync(symptoms);

                // // 🔍 Match against dictionary
                // var matchedDepartment = _symptomDepartmentMap
                //     .FirstOrDefault(kvp => symptoms.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                //     .Value;

                if (string.IsNullOrWhiteSpace(matchedDepartment))
                {
                    _logger.LogInformation("No department found for symptoms '{Symptoms}'", symptoms);
                    return CreateError(call.Id, $"⚠️ Could not resolve speciality for symptoms: {symptoms}");
                }

                // 🏷️ Convert to enum
                if (!Enum.TryParse(typeof(SpecialityEnum), matchedDepartment, true, out var matchedEnum))
                {
                    _logger.LogWarning("Mapped department '{Dept}' not found in SpecialityEnum", matchedDepartment);
                    return CreateError(call.Id, $"⚠️ No matching speciality enum found for department: {matchedDepartment}");
                }

                var specialityEnum = (SpecialityEnum)matchedEnum;

                // 🧑‍⚕️ Example provider object (mocked for now)
                var result = new
                {
                    success = true,
                    symptoms,
                    department = matchedDepartment,
                    specialityId = (int)specialityEnum,
                    specialityName = specialityEnum.ToString()
                };

                _logger.LogInformation(
                    "Resolved symptoms '{Symptoms}' → Department '{Dept}' → SpecialityEnum '{Enum}'",
                    symptoms, matchedDepartment, specialityEnum);

                return CreateSuccess(call.Id, "✅ Speciality resolved successfully.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in ResolveDoctorSpecialityToolHandler");
                return CreateError(call.Id, "❌ An internal error occurred while resolving doctor speciality.");
            }
        }
    }
}

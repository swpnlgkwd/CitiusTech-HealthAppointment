using System.Threading.Tasks;

namespace CitiusTech_HealthAppointmentApis.Agent.Services
{
    /// <summary>
    /// Contract for resolving patient symptoms to a doctor department/speciality.
    /// </summary>
    public interface IDoctorSpecialityResolverService
    {
        /// <summary>
        /// Resolve symptoms into a medical department (string name of speciality).
        /// </summary>
        /// <param name="symptoms">Raw symptom text provided by patient</param>
        /// <returns>Department name (must match SpecialityEnum) or null if not resolved</returns>
        Task<string?> ResolveDepartmentAsync(string symptoms);
    }
}

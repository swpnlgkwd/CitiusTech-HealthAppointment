using PatientAppointments.Core.Contracts.Repositories.Base;
using PatientAppointments.Core.Entities;
using PatientAppointments.Core.Entities.Risk;

namespace PatientAppointments.Core.Contracts.Repositories
{
    public interface IPatientRiskFactorRepository : IGenericRepository<PatientRiskFactor>
    {
        // Add custom query methods here if needed in future
    }
}

using PatientAppointments.Core.Entities;
using PatientAppointments.Core.Contracts.Repositories.Base;
using PatientAppointments.Core.Entities.Risk;

namespace PatientAppointments.Core.Contracts.Repositories
{
    public interface IPatientRiskScoreRepository : IGenericRepository<PatientRiskScore>
    {
        // Add custom methods if needed in the future
    }
}

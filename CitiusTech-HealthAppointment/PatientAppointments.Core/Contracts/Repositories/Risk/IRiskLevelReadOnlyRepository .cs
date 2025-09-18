using PatientAppointments.Core.Entities;
using PatientAppointments.Core.Contracts.Repositories.Base;
using PatientAppointments.Core.Entities.Risk;

namespace PatientAppointments.Core.Contracts.Repositories
{
    public interface IRiskLevelReadOnlyRepository : IReadOnlyRepositoryBase<RiskLevel>
    {
        // Add any custom read-only queries if needed
    }
}

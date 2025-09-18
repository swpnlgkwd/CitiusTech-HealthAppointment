using PatientAppointments.Core.Entities;
using PatientAppointments.Core.Contracts.Repositories.Base;
using PatientAppointments.Core.Entities.Risk;

namespace PatientAppointments.Core.Contracts.Repositories.Risk
{
    public interface IRiskTypeReadOnlyRepository : IReadOnlyRepositoryBase<RiskType>
    {
        // Add any custom read-only queries if needed
    }
}

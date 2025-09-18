using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Contracts.Repositories.Risk;
using PatientAppointments.Core.Entities;
using PatientAppointments.Core.Entities.Risk;
using PatientAppointments.Infrastructure.Data;
using PatientAppointments.Infrastructure.Repositories.Base;

namespace PatientAppointments.Infrastructure.Repositories
{
    public class RiskTypeReadOnlyRepository : ReadOnlyRepositoryBase<RiskType>, IRiskTypeReadOnlyRepository
    {
        public RiskTypeReadOnlyRepository(AppDbContext context) : base(context) { }
    }
}

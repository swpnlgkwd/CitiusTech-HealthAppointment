using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Core.Entities.Risk;
using PatientAppointments.Infrastructure.Data;
using PatientAppointments.Infrastructure.Repositories.Base;

namespace PatientAppointments.Infrastructure.Repositories
{
    public class RiskLevelReadOnlyRepository : ReadOnlyRepositoryBase<RiskLevel>, IRiskLevelReadOnlyRepository
    {
        public RiskLevelReadOnlyRepository(AppDbContext context) : base(context) { }
    }
}

using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;

namespace PatientAppointments.Infrastructure.Repositories {
    public class ProviderScheduleRepository : GenericRepository<ProviderSchedule>, IProviderScheduleRepository
    {
        public ProviderScheduleRepository(AppDbContext ctx) : base(ctx) { }
    }
}

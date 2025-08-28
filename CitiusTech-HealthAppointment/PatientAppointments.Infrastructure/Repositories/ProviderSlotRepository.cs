using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;

namespace PatientAppointments.Infrastructure.Repositories {
    public class ProviderSlotRepository : GenericRepository<ProviderSlot>, IProviderSlotRepository
    {
        public ProviderSlotRepository(AppDbContext ctx) : base(ctx) { }
    }
}

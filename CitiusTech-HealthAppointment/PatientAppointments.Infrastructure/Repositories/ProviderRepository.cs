using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;

namespace PatientAppointments.Infrastructure.Repositories {   

    public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(AppDbContext ctx) : base(ctx) { }
    }
}

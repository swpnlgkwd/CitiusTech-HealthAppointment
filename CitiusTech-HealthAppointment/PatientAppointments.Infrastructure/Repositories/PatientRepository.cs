using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;

namespace PatientAppointments.Infrastructure.Repositories {
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository {
        public PatientRepository(AppDbContext ctx) : base(ctx) { }
    }
}

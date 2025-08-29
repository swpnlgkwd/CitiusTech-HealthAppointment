using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;
using PatientAppointments.Infrastructure.Repositories.Base;

namespace PatientAppointments.Infrastructure.Repositories
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository {
        public PatientRepository(AppDbContext ctx) : base(ctx) { }
    }
}

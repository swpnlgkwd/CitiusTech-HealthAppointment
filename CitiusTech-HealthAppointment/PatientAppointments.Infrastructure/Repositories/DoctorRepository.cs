using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;

namespace PatientAppointments.Infrastructure.Repositories {
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository {
        public DoctorRepository(AppDbContext ctx) : base(ctx) { }
    }
}

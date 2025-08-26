using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;

namespace PatientAppointments.Infrastructure.Repositories {
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository {
        public AppointmentRepository(AppDbContext ctx) : base(ctx) { }
    }
}

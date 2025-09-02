using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;
using PatientAppointments.Infrastructure.Repositories.Base;

namespace PatientAppointments.Infrastructure.Repositories
{
    public class AppointmentTypeRepository : GenericRepository<AppointmentType>, IAppointmentTypeRepository
    {
        public AppointmentTypeRepository(AppDbContext ctx) : base(ctx) { }
    }
}

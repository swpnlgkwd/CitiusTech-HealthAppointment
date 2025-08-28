using PatientAppointments.Core.Contracts;
using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Infrastructure.Data;
using System.Threading.Tasks;

namespace PatientAppointments.Infrastructure {
    public class UnitOfWork : IUnitOfWork {

        private readonly AppDbContext _ctx;
        public IPatientRepository Patients { get; }
        public IDoctorRepository Doctors { get; }
        public IAppointmentRepository Appointments { get; }
        public UnitOfWork(AppDbContext ctx, IPatientRepository p, IDoctorRepository d, IAppointmentRepository a) {
            _ctx = ctx; Patients = p; Doctors = d; Appointments = a;
        }
        public Task<int> CompleteAsync() => _ctx.SaveChangesAsync();
    }
}

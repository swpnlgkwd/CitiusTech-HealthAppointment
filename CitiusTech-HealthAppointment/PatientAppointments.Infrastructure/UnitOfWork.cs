using PatientAppointments.Core.Contracts;
using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Infrastructure.Data;
using System.Threading.Tasks;

namespace PatientAppointments.Infrastructure {
    public class UnitOfWork : IUnitOfWork {

        private readonly AppDbContext _ctx;
        public IPatientRepository Patients { get; }
        public IProviderRepository Provider { get; }
        public IAppointmentRepository Appointments { get; }
        public IAppointmentTypeRepository AppointmentsType { get; }
        public IProviderSlotRepository ProviderSlot { get; }
        public IProviderScheduleRepository ProviderSchedule { get; }

        public UnitOfWork(AppDbContext ctx, IPatientRepository p, IProviderRepository d, IAppointmentRepository a,
            IProviderSlotRepository ProviderSlot, IProviderScheduleRepository ProviderSchedule)
        {
            _ctx = ctx; 
            Patients = p; 
            Provider = d; 
            Appointments = a;
            this.ProviderSchedule = ProviderSchedule;
            this.ProviderSlot = ProviderSlot;
        }
        public Task<int> CompleteAsync() => _ctx.SaveChangesAsync();
    }
}

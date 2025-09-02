using PatientAppointments.Core.Contracts.Repositories;
using System.Threading.Tasks;

namespace PatientAppointments.Core.Contracts {
    public interface IUnitOfWork {
        IPatientRepository Patients { get; }
        IProviderRepository Provider { get; }
        IAppointmentRepository Appointments { get; }
        IAppointmentTypeRepository AppointmentsType { get; }
        IProviderScheduleRepository ProviderSchedule { get; }
        IProviderSlotRepository ProviderSlot { get; }
        Task<int> CompleteAsync();
    }
}

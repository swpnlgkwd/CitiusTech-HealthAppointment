using PatientAppointments.Core.Contracts.Repositories;
using System.Threading.Tasks;

namespace PatientAppointments.Core.Contracts {
    public interface IUnitOfWork {
        IPatientRepository Patients { get; }
        IDoctorRepository Doctors { get; }
        IAppointmentRepository Appointments { get; }
        Task<int> CompleteAsync();
    }
}

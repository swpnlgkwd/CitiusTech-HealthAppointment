using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Contracts.Repositories.Risk;
using System.Threading.Tasks;

namespace PatientAppointments.Core.Contracts {
    public interface IUnitOfWork {
        IPatientRepository Patients { get; }
        IProviderRepository Provider { get; }
        IAppointmentRepository Appointments { get; }
        IAppointmentTypeRepository AppointmentsType { get; }
        IProviderScheduleRepository ProviderSchedule { get; }
        IProviderSlotRepository ProviderSlot { get; }
        ISpecialityRepository Speciality { get; }

        IPatientRiskFactorRepository PatientRiskFactorRepository { get; }

        IPatientRiskScoreRepository PatientRiskScoreRepository { get; }

        IRiskLevelReadOnlyRepository RiskLevelReadOnlyRepository { get; }

        IRiskTypeReadOnlyRepository RiskTypeReadOnlyRepository { get; }

        Task<int> CompleteAsync();
    }
}

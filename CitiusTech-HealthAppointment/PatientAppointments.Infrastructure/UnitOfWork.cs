using PatientAppointments.Core.Contracts;
using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Contracts.Repositories.Risk;
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
        public ISpecialityRepository Speciality { get; }
        public IAgentConversationsRepository AgentConversations { get; }

        public IPatientRiskFactorRepository PatientRiskFactorRepository { get; }

        public IPatientRiskScoreRepository PatientRiskScoreRepository { get; }

        public IRiskLevelReadOnlyRepository RiskLevelReadOnlyRepository { get; }

        public IRiskTypeReadOnlyRepository RiskTypeReadOnlyRepository { get; }

        public UnitOfWork(AppDbContext ctx, IPatientRepository p, IProviderRepository d, IAppointmentRepository a,
            IProviderSlotRepository ProviderSlot, IProviderScheduleRepository ProviderSchedule, IAppointmentTypeRepository appointmentType, ISpecialityRepository Speciality,
            IAgentConversationsRepository agentConversationsRepository,
            IPatientRiskFactorRepository patientRiskFactorRepository,
            IPatientRiskScoreRepository patientRiskScoreRepository,
            IRiskLevelReadOnlyRepository riskLevelReadOnlyRepository,
            IRiskTypeReadOnlyRepository riskTypeReadOnlyRepository)
        {
            _ctx = ctx; 
            Patients = p; 
            Provider = d; 
            Appointments = a;
            this.ProviderSchedule = ProviderSchedule;
            this.ProviderSlot = ProviderSlot;
            this.AppointmentsType = appointmentType;
            this.Speciality = Speciality;
            this.AgentConversations = agentConversationsRepository;
            this.PatientRiskScoreRepository = patientRiskScoreRepository;
            this.PatientRiskFactorRepository = patientRiskFactorRepository;
            this.RiskLevelReadOnlyRepository = riskLevelReadOnlyRepository;
            this.RiskTypeReadOnlyRepository = riskTypeReadOnlyRepository;
        }
        public Task<int> CompleteAsync() => _ctx.SaveChangesAsync();
    }
}

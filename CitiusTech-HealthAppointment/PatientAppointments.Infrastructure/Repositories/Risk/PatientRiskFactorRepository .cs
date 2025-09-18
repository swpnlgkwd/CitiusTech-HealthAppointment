using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Core.Entities.Risk;
using PatientAppointments.Infrastructure.Data;
using PatientAppointments.Infrastructure.Repositories.Base;

namespace PatientAppointments.Infrastructure.Repositories
{
    public class PatientRiskFactorRepository : GenericRepository<PatientRiskFactor>, IPatientRiskFactorRepository
    {
        public PatientRiskFactorRepository(AppDbContext ctx) : base(ctx)
        {
        }

        // Add any custom methods for PatientRiskFactor here if needed
    }
}

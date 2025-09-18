using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Core.Entities.Risk;
using PatientAppointments.Infrastructure.Data;
using PatientAppointments.Infrastructure.Repositories.Base;

namespace PatientAppointments.Infrastructure.Repositories
{
    public class PatientRiskScoreRepository : GenericRepository<PatientRiskScore>, IPatientRiskScoreRepository
    {
        public PatientRiskScoreRepository(AppDbContext ctx) : base(ctx)
        {
        }

        // Add custom query or update logic here if needed
    }
}

using Microsoft.EntityFrameworkCore;
using PatientAppointments.Business.Contracts.Risk;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Contracts;
using PatientAppointments.Core.Entities.Risk;

namespace PatientAppointments.Business.Services.Risk
{
    public class PatientRiskManager : IPatientRiskManager
    {
        private readonly IUnitOfWork _uow;
        public PatientRiskManager(IUnitOfWork uow)
        {
            this._uow = uow;
        }

        public async Task<PatientRiskDto?> GetPatientWithRisksAsync(int patientId)
        {
            // 1. Fetch patient info
            var patient = await _uow.Patients.GetByIdAsync(patientId);
            if (patient == null) return null;

            // 2. Fetch patient risk factors (non-deleted)
            var riskFactors = await _uow.PatientRiskFactorRepository.FindAsync(rf => rf.PatientId == patientId && rf.IsDeleted == false);

            // 3. Extract distinct RiskTypeIds and RiskLevelIds from risk factors
            var riskTypeIds = riskFactors.Select(rf => rf.RiskTypeId).Distinct().ToList();
            var riskLevelIds = riskFactors.Select(rf => rf.RiskLevelId).Distinct().ToList();

            // 4. Fetch RiskTypes by IDs
            var riskTypes = await _uow.RiskTypeReadOnlyRepository.Query()
                .Where(rt => riskTypeIds.Contains(rt.RiskTypeId))
                .ToListAsync();

            // 5. Fetch RiskLevels by IDs
            var riskLevels = await _uow.RiskLevelReadOnlyRepository.Query()
                .Where(rl => riskLevelIds.Contains(rl.RiskLevelId))
                .ToListAsync();

            // 6. Fetch patient's risk scores (non-deleted), get latest
            var riskScores = await _uow.PatientRiskScoreRepository.FindAsync(rs => rs.PatientId == patientId);
            var latestRiskScore = riskScores.OrderByDescending(rs => rs.CalculatedAt).FirstOrDefault();

            // 7. If risk score found, get its RiskLevel
            RiskLevel? riskScoreLevel = null;
            if (latestRiskScore != null)
            {
                riskScoreLevel = await _uow.RiskLevelReadOnlyRepository.Query()
                    .Where(rl => rl.RiskLevelId == latestRiskScore.RiskLevelId)
                    .FirstOrDefaultAsync();
            }

            // 8. Map data to DTO
            var dto = new PatientRiskDto
            {
                PatientId = patient.PatientId,
                PatientName = patient.FullName, // Change this if your Patient entity uses a different property
                RiskFactors = riskFactors.Select(rf => new RiskFactorDto
                {
                    RiskFactorId = rf.RiskId,
                    RiskTypeName = riskTypes.FirstOrDefault(rt => rt.RiskTypeId == rf.RiskTypeId)?.RiskTypeName ?? "Unknown",
                    RiskLevelName = riskLevels.FirstOrDefault(rl => rl.RiskLevelId == rf.RiskLevelId)?.RiskLevelName ?? "Unknown",
                    IdentifiedOn = rf.IdentifiedOn
                }).ToList(),
                RiskScore = latestRiskScore == null ? null : new RiskScoreDto
                {
                    Score = latestRiskScore.Score,
                    RiskLevelName = riskScoreLevel?.RiskLevelName ?? "Unknown",
                    Reason = latestRiskScore.Reason,
                    CalculatedAt = latestRiskScore.CalculatedAt
                }
            };

            return dto;
        }
    }
}

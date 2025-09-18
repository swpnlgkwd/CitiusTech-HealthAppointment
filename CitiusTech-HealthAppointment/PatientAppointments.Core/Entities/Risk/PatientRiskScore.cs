using System;

namespace PatientAppointments.Core.Entities.Risk
{
    public class PatientRiskScore
    {
        public int PatientId { get; set; }

        public int Score { get; set; }  // 0–100

        public int RiskLevelId { get; set; }

        public string? Reason { get; set; }

        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

    }
}

namespace PatientAppointments.Business.Dtos
{
    public class RiskScoreDto
    {
        public int Score { get; set; }
        public string RiskLevelName { get; set; }
        public string? Reason { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}

namespace PatientAppointments.Business.Dtos
{
    public class PatientRiskDto
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } // Assuming Patient has Name
        public List<RiskFactorDto> RiskFactors { get; set; } = new List<RiskFactorDto>();
        public RiskScoreDto? RiskScore { get; set; }
    }
}

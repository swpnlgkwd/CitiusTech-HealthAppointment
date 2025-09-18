namespace PatientAppointments.Business.Dtos
{
    public class RiskFactorDto
    {
        public int RiskFactorId { get; set; }
        public string RiskTypeName { get; set; }
        public string RiskLevelName { get; set; }
        public DateTime IdentifiedOn { get; set; }
    }
}

namespace CitiusTech_HealthAppointmentApis.Dto
{
    public class AppointmentInfoDto
    {
        public int id { get; set; }
        public string doctor { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public DateTime date { get; set; }
        public string status { get; set; } = string.Empty;
    }
}

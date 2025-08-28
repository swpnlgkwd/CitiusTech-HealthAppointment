namespace PatientAppointments.Core.Entities {
    public class Provider : BaseEntity {

        public int ProviderId { get; set; }
        public string FullName { get; set; }
        public int? SpecialtyId { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public bool IsActive { get; set; }
    }
}

namespace PatientAppointments.Core.Entities {
    public class Doctor : BaseEntity {
        public int DoctorId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

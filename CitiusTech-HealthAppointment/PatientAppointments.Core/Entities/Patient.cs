using System;

namespace PatientAppointments.Core.Entities {
    public class Patient : BaseEntity {
        public int PatientId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

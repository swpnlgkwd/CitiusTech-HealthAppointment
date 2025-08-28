using System;

namespace PatientAppointments.Core.Entities {
    public class Patient : BaseEntity {
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public DateTime? Dob { get; set; }
        public int? GenderId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace PatientAppointments.Infrastructure.Identity {
    public class ApplicationUser : IdentityUser {
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using PatientAppointments.Infrastructure.Identity;

namespace PatientAppointments.Infrastructure.Identity {
    public class RefreshToken {
        [Key] public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; } = string.Empty;
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= Expires;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = default!;
    }
}

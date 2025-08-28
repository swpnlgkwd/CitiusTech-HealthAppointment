using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PatientAppointments.Core.Entities {

    public class Appointment : BaseEntity {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }

        public int PatientId { get; set; }

        public int ProviderId { get; set; }

        public int SlotId { get; set; }

        public DateTime StartUtc { get; set; }

        public DateTime EndUtc { get; set; }

        public int StatusId { get; set; }

        public int TypeId { get; set; }

        // Extra fields
        public string? Notes { get; set; }

        public bool ReminderSent { get; set; } = false;

        public DateTime? ReminderSentAt { get; set; }

    }
}

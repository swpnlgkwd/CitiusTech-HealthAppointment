using System;

namespace PatientAppointments.Core.Entities {
    public class ProviderSlot : BaseEntity {
        public int SlotId { get; set; }
        public int ScheduleId { get; set; }
        public TimeSpan SlotStart { get; set; }
        public TimeSpan SlotEnd { get; set; }
        public bool IsBooked { get; set; }
    }
}

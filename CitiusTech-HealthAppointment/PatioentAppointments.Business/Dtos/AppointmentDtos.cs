using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Dtos
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int ProviderId { get; set; }
        public int SlotId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public string? Notes { get; set; } = string.Empty;
    }
    
}

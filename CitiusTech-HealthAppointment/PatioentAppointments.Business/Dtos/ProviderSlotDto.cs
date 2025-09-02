using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Dtos
{
    public class ProviderSlotDto
    {
        public int Id { get; set; }

        public int providerId { get; set; }

        public DateTime scheduleDate {  get; set; }

        public TimeSpan startTime { get; set; }

        public TimeSpan endTime { get; set; }

        public bool isBooked { get; set; }
    }
}

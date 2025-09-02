using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Enums
{
    public enum AppointmentStatus
    {
        Booked = 1,
        Rescheduled = 2,
        Cancelled = 3,
        Completed = 4,
        NoShow = 5
    }
}

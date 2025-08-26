using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Dtos
{
    public record AppointmentDto(int Id, int DoctorId, int PatientId, DateTime ScheduledAt, string Status);

    public record CreateAppointmentDto(int DoctorId, int PatientId, DateTime ScheduledAt);

    public record UpdateAppointmentDto(int Id, DateTime ScheduledAt, string Status);
}

using PatientAppointments.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Contracts
{
    public interface IAppointmentManager
    {
        Task<AppointmentDto> CreateAsync(AppointmentDto dto);
        Task<IEnumerable<AppointmentDto>> GetByDoctorAsync(int doctorId);
        Task<IEnumerable<AppointmentDto>> GetByPatientAsync(int patientId);
        Task<AppointmentDto> UpdateAsync(AppointmentDto dto);
        Task<bool> CancelAsync(int id);
    }
}

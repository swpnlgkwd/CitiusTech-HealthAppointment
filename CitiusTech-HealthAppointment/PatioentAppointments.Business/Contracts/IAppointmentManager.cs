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
        Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
        Task<IEnumerable<AppointmentDto>> GetByDoctorAsync(Guid doctorId);
        Task<IEnumerable<AppointmentDto>> GetByPatientAsync(Guid patientId);
        Task<AppointmentDto> UpdateAsync(UpdateAppointmentDto dto);
        Task<bool> CancelAsync(Guid id);
    }
}

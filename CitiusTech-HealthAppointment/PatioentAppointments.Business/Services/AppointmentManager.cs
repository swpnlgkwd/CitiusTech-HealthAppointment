using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Contracts;
using PatientAppointments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Services
{
    public class AppointmentManager : IAppointmentManager
    {
        private readonly IUnitOfWork _uow;

        public AppointmentManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
        {
            var appointment = new Appointment
            {
                AppointmentId = 0,
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                AppointmentDate = dto.ScheduledAt,
                Status = "Scheduled"
            };

            await _uow.Appointments.AddAsync(appointment);
            await _uow.CompleteAsync();

            return new AppointmentDto(appointment.AppointmentId, appointment.DoctorId, appointment.PatientId, appointment.AppointmentDate, appointment.Status);
        }

        public async Task<IEnumerable<AppointmentDto>> GetByDoctorAsync(Guid doctorId)
        {
            throw new NotImplementedException("This method is not implemented yet. Please implement it according to your requirements.");
        }

        public async Task<IEnumerable<AppointmentDto>> GetByPatientAsync(Guid patientId)
        {
            throw new NotImplementedException("This method is not implemented yet. Please implement it according to your requirements.");
        }

        public async Task<AppointmentDto> UpdateAsync(UpdateAppointmentDto dto)
        {
            throw new NotImplementedException("This method is not implemented yet. Please implement it according to your requirements.");
        }

        public async Task<bool> CancelAsync(Guid id)
        {
            throw new NotImplementedException("This method is not implemented yet. Please implement it according to your requirements.");
        }
    }
}

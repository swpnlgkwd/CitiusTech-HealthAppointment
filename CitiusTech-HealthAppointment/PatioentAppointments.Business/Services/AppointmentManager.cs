using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Contracts;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Business.Services
{
    public class AppointmentManager : IAppointmentManager
    {
        private readonly IUnitOfWork _uow;

        public AppointmentManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AppointmentDto> CreateAsync(AppointmentDto dto)
        {
            var appointment = new Appointment
            {
                AppointmentId = 0,
                ProviderId = dto.ProviderId,
                PatientId = dto.PatientId,
                SlotId = dto.SlotId,
                StatusId = 0,
                StartUtc = dto.StartUtc,
                EndUtc = dto.EndUtc,
                Notes = dto.Notes,
                TypeId = dto.TypeId
            };
            await _uow.Appointments.AddAsync(appointment);
            await _uow.CompleteAsync();
            return dto;
        }

        public async Task<IEnumerable<AppointmentDto>> GetByDoctorAsync(int doctorId)
        {
            throw new NotImplementedException("This method is not implemented yet. Please implement it according to your requirements.");
        }

        public async Task<IEnumerable<AppointmentDto>> GetByPatientAsync(int patientId)
        {
            throw new NotImplementedException("This method is not implemented yet. Please implement it according to your requirements.");
        }

        public async Task<AppointmentDto> UpdateAsync(AppointmentDto dto)
        {
            throw new NotImplementedException("This method is not implemented yet. Please implement it according to your requirements.");
        }

        public async Task<bool> CancelAsync(int id)
        {
            throw new NotImplementedException("This method is not implemented yet. Please implement it according to your requirements.");
        }
    }
}

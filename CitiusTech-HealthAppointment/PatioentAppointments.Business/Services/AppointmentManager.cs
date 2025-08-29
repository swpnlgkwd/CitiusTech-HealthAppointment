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

            // update dto with generated Id
            dto.AppointmentId = appointment.AppointmentId;
            return dto;
        }

        public async Task<IEnumerable<AppointmentDto>> GetByDoctorAsync(int doctorId)
        {
            var appointments = await _uow.Appointments
                .FindAsync(a => a.ProviderId == doctorId);

            return appointments.Select(MapToDto);
        }

        public async Task<IEnumerable<AppointmentDto>> GetByPatientAsync(int patientId)
        {
            var appointments = await _uow.Appointments
                .FindAsync(a => a.PatientId == patientId);

            return appointments.Select(MapToDto);
        }

        public async Task<AppointmentDto> UpdateAsync(AppointmentDto dto)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(dto.AppointmentId);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment {dto.AppointmentId} not found");

            appointment.ProviderId = dto.ProviderId;
            appointment.PatientId = dto.PatientId;
            appointment.SlotId = dto.SlotId;
            appointment.StatusId = dto.StatusId;
            appointment.StartUtc = dto.StartUtc;
            appointment.EndUtc = dto.EndUtc;
            appointment.Notes = dto.Notes;
            appointment.TypeId = dto.TypeId;

            _uow.Appointments.Update(appointment);
            await _uow.CompleteAsync();

            return MapToDto(appointment);
        }

        public async Task<bool> CancelAsync(int id)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(id);
            if (appointment == null)
                return false;

            //soft cancel by updating status
            appointment.StatusId = (int)AppointmentStatus.Cancelled;
            _uow.Appointments.Update(appointment);

            await _uow.CompleteAsync();
            return true;
        }

        private static AppointmentDto MapToDto(Appointment a)
        {
            return new AppointmentDto
            {
                AppointmentId = a.AppointmentId,
                ProviderId = a.ProviderId,
                PatientId = a.PatientId,
                SlotId = a.SlotId,
                StatusId = a.StatusId,
                StartUtc = a.StartUtc,
                EndUtc = a.EndUtc,
                Notes = a.Notes,
                TypeId = a.TypeId
            };
        }
    }


    public enum AppointmentStatus
    {
        Scheduled = 0,
        Completed = 1,
        Cancelled = 2,
        NoShow = 3
    }
}

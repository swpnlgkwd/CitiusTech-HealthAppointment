using CitiusTech_HealthAppointmentApis.Dto;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Contracts;
using PatientAppointments.Core.Entities;
using PatientAppointments.Business.Enums;

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
                StatusId = dto.StatusId,
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

        public async Task<AppointmentDto> GetByIdAsync(int appintmentId)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(appintmentId);
            if (appointment == null)
                return null;
            return MapToDto(appointment);
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

        public async Task<bool> CancelAsync(int id, string note)
        {
            var appointment = await _uow.Appointments.GetByIdAsync(id);
            if (appointment == null)
                return false;

            //soft cancel by updating status
            appointment.StatusId = (int)AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.UtcNow;
            appointment.Notes = note;
            _uow.Appointments.Update(appointment);

            await _uow.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<AppointmentTypeDto>> GetAppointmentTypesAsync()
        {
            var appTypeList = await _uow.AppointmentsType.GetAllAsync();
            return appTypeList.Select(x => new AppointmentTypeDto { type_id = x.type_id, type_name = x.type_name });
        }

        public async Task<IEnumerable<ProviderSlotDto>> GetProviderSlotsAsync(int ProviderId, DateTime sDate, DateTime? eDate)
        {
            var slots = await _uow.ProviderSlot.GetAllAsync();
            var schedule = await _uow.ProviderSchedule.GetAllAsync();
            return schedule.Where(p => p.ProviderId == ProviderId && p.ScheduleDate >= sDate && p.ScheduleDate <=eDate)
                .Join(slots, sc => sc.ScheduleId, sl => sl.ScheduleId,
                (sc, sl) => new ProviderSlotDto { Id = sc.ScheduleId, providerId = sc.ProviderId, scheduleDate = sc.ScheduleDate, startTime = sl.SlotStart, endTime = sl.SlotEnd, isBooked = sl.IsBooked });
        }

        public async Task<IEnumerable<AppointmentInfoDto>> FetchAppointmentAsync(int userId, string role)
        {
            IEnumerable<AppointmentDto> appointments = [];
            if(role.Equals("Patient", StringComparison.OrdinalIgnoreCase))
            {
                appointments = await GetByPatientAsync(userId);
            }
            else
            {
                appointments = await GetByDoctorAsync(userId);
            };

            if (appointments == null || !appointments.Any())
            {
                return Enumerable.Empty<AppointmentInfoDto>();
            }

            //get all doctors, patients and appointment types
            var providers = await _uow.Provider.GetAllAsync();
            var patients = await _uow.Patients.GetAllAsync();
            var appTypes = await _uow.AppointmentsType.GetAllAsync();

            //join doctor, patient, appointmentType with appointments to get the required info
            var result = from a in appointments
                         join d in providers on a.ProviderId equals d.ProviderId
                         join p in patients on a.PatientId equals p.PatientId
                         join t in appTypes on a.TypeId equals t.type_id
                         select new AppointmentInfoDto
                         {
                             id = a.AppointmentId,
                             doctor = d.FullName,
                             type = t.type_name,
                             date = a.StartUtc,
                             status = ((AppointmentStatus)a.StatusId).ToString()
                         };

            return result.ToList();
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
}

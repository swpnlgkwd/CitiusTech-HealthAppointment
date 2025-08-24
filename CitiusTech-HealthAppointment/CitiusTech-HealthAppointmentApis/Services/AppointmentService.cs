using CitiusTech_HealthAppointmentApis.Common.Exceptions;
using CitiusTech_HealthAppointmentApis.Services.Interfaces;

namespace CitiusTech_HealthAppointmentApis.Services
{
    public class AppointmentService : IAppointmentService
    {
        public async Task<AppointmentResult?> AppointmentBookingAsync(int patientId, int providerId, int slotId, int statusId, int typeId, string? notes)
        {
            // Simulate slot lookup and booking logic
            // In real code, query ProviderSlots and Appointment tables

            // Example: Check if slot is available
            bool slotAvailable = true; // Replace with DB check

            if (!slotAvailable)
                throw new BusinessRuleException("Selected slot is already booked.");

            // Simulate appointment creation
            var startUtc = DateTime.UtcNow.AddDays(1); // Replace with slot start time
            var endUtc = startUtc.AddMinutes(60);      // Replace with slot end time

            return new AppointmentResult
            {
                AppointmentId = new Random().Next(1000, 9999),
                PatientId = patientId,
                ProviderId = providerId,
                SlotId = slotId,
                StartUtc = startUtc,
                EndUtc = endUtc,
                StatusId = statusId,
                TypeId = typeId,
                Confirmation = "Confirmed"
            };
        }
    }
}
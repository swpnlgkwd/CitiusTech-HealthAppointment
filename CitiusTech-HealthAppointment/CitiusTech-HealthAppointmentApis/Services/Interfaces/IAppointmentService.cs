namespace CitiusTech_HealthAppointmentApis.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentResult?> AppointmentBookingAsync(int patientId, int providerId, int slotId, int statusId, int typeId, string? notes);
    }

    public class AppointmentResult
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int ProviderId { get; set; }
        public int SlotId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public string Confirmation { get; set; } = string.Empty;
    }
}
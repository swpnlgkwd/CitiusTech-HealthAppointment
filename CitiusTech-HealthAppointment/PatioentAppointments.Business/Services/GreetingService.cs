using Microsoft.AspNetCore.Http;
using PatientAppointments.Core.Contracts;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace PatientAppointments.Business.Services
{
    public class GreetingService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _uow;

        public GreetingService(IHttpContextAccessor httpContextAccessor, IUnitOfWork uow)
        {
            _httpContextAccessor = httpContextAccessor;
            _uow = uow;
        }

        public async Task<string> GetGreetingAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
                return "Hello Guest, please log in to continue.";

            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
            var name = role == "Provider" ? await GetProviderName(user) : await GetPatientName(user);

            var baseGreeting = $"{GetTimeBasedGreeting()} {name}, ";

            switch (role)
            {
                case "Admin":
                    return baseGreeting + "as an Admin, you can manage users, appointments, and system settings.";

                case "Provider":
                    return baseGreeting + await GetProviderMessageAsync(user);

                case "Patient":
                    return baseGreeting + await GetPatientMessageAsync(user);

                default:
                    return baseGreeting + "welcome to the system!";
            }
        }

        private async Task<string> GetProviderMessageAsync(ClaimsPrincipal user)
        {
            var providerIdClaim = user.FindFirst("ProviderId")?.Value;
            if (providerIdClaim == null) return "your schedule is ready.";

            int providerId = int.Parse(providerIdClaim);

            var upcoming = await (
                 from a in _uow.Appointments.Query()
                 join p in _uow.Patients.Query() on a.PatientId equals p.PatientId
                 where a.ProviderId == providerId && a.CreatedAt >= DateTime.Today
                 orderby a.CreatedAt
                 select new
                 {
                     AppointmentDate = a.CreatedAt,
                     PatientName = p.FullName
                 }
             ).FirstOrDefaultAsync();

            if (upcoming == null)
                return "you don’t have any upcoming appointments.";

            return $"your next appointment is with patient {upcoming.PatientName} on {upcoming.AppointmentDate:g}.";

        }

        private async Task<string> GetPatientMessageAsync(ClaimsPrincipal user)
        {
            var patientIdClaim = user.FindFirst("PatientId")?.Value;
            if (patientIdClaim == null) return "you can view or book appointments easily.";

            int patientId = int.Parse(patientIdClaim);

            var upcoming = await (
                from a in _uow.Appointments.Query()   // <-- expose IQueryable from repo
                join p in _uow.Provider.Query() on a.ProviderId equals p.ProviderId
                where a.PatientId == patientId && a.CreatedAt >= DateTime.Today
                orderby a.CreatedAt
                select new
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.CreatedAt,
                    ProviderName = p.FullName
                }
            ).FirstOrDefaultAsync();


            if (upcoming == null)
                return "you have no upcoming appointments. Book one today!";

            return $"your next appointment is with Dr. {upcoming.ProviderName} on {upcoming.AppointmentDate:g}.";
        }


        private async Task<string> GetProviderName(ClaimsPrincipal user)
        {
            var providerIdClaim = user.FindFirst("ProviderId")?.Value;
            if (providerIdClaim == null) return "your schedule is ready.";

            int providerId = int.Parse(providerIdClaim);

            var provider = await _uow.Provider.GetByIdAsync(providerId);

            if (provider == null)
                return "User";

            return $"{provider.FullName}.";

        }

        private async Task<string> GetPatientName(ClaimsPrincipal user)
        {
            var providerIdClaim = user.FindFirst("Patientid")?.Value;
            if (providerIdClaim == null) return "your schedule is ready.";

            int patientId = int.Parse(providerIdClaim);

            var patient = await _uow.Patients.GetByIdAsync(patientId);

            if (patient == null)
                return "User";

            return $"{patient.FullName}.";

        }

        private string GetTimeBasedGreeting()
        {
            var hour = DateTime.Now.Hour;

            if (hour < 12) return "Good morning";
            if (hour < 18) return "Good afternoon";
            return "Good evening";
        }
    }

}

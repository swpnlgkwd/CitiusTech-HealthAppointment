using Microsoft.AspNetCore.Http;
using PatientAppointments.Core.Contracts;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PatientAppointments.Business.Dtos;

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

            return $"{GetTimeBasedGreeting()} {name}";
            
        }


        public async Task<AgentSummaryResponseDto?> GetDailySummaryAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
            var name = role == "Provider" ? await GetProviderName(user) : await GetPatientName(user);

            string greeting = await GetGreetingAsync();

            greeting = $"{greeting}, welcome to MediMate!";

            switch (role)
            {
                case "Admin":
                    return new AgentSummaryResponseDto
                    {
                        SummaryMessage = greeting,
                        QuickReplies = new List<QuickReplyDto>()
                    };

                case "Provider":
                    return await GetDoctorSummaryAsync(greeting);

                case "Patient":
                    return await GetPatientSummaryAsync(greeting);

                default:
                    return null;
            }           

        }

        #region Helpers

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

            return $"{provider.FullName}";

        }

        private async Task<string> GetPatientName(ClaimsPrincipal user)
        {
            var providerIdClaim = user.FindFirst("Patientid")?.Value;
            if (providerIdClaim == null) return "your schedule is ready.";

            int patientId = int.Parse(providerIdClaim);

            var patient = await _uow.Patients.GetByIdAsync(patientId);

            if (patient == null)
                return "User";

            return $"{patient.FullName}";

        }

        private string GetTimeBasedGreeting()
        {
            var hour = DateTime.Now.Hour;

            if (hour < 12) return "Good morning";
            if (hour < 18) return "Good afternoon";
            return "Good evening";
        }        


        private async Task<AgentSummaryResponseDto> GetDoctorSummaryAsync(string greeting)
        {
            var today = DateTime.Today;
            var user = _httpContextAccessor.HttpContext?.User;
            var ProviderIdClaim = user.FindFirst("ProviderId")?.Value;
            int ProviderId = int.Parse(ProviderIdClaim);
            var appointments = await _uow.Appointments.Query()
                .Where(s => s.ProviderId == ProviderId)
                .ToListAsync();

            var message = appointments.Any()
                ? $"{greeting}, you have {appointments.Count} appointment(s) scheduled for today."
                : $"{greeting}, you don’t have any appointments today.";

            var quickReplies = new List<QuickReplyDto>
            {
                new QuickReplyDto { Label = "👨‍⚕️ View Today’s Patients", Value = "Show my patients today" },
                new QuickReplyDto { Label = "🗓 Manage Schedule", Value = "Open my schedule" }
            };

            return new AgentSummaryResponseDto { SummaryMessage = message, QuickReplies = quickReplies };
        }

        private async Task<AgentSummaryResponseDto> GetPatientSummaryAsync(string greeting)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var PatientIdClaim = user.FindFirst("PatientId")?.Value;
            int PatientId = int.Parse(PatientIdClaim);
            var upcoming = await _uow.Appointments.Query()
                .Where(s => s.PatientId == PatientId)
                .OrderBy(a => a.StartUtc)
                .ToListAsync();

            var nextAppt = upcoming.OrderBy(a => a.StartUtc).FirstOrDefault();

            var provider = await _uow.Provider.FindAsync(s => s.ProviderId == nextAppt.ProviderId);

            var message = nextAppt != null
                ? $"{greeting}, your next appointment is with Dr. {provider.FirstOrDefault().FullName} on {nextAppt.StartUtc:g}."
                : $"{greeting}, you don’t have any upcoming appointments.";

            var quickReplies = new List<QuickReplyDto>
            {
                new QuickReplyDto { Label = "📅 View My Appointments", Value = "Show my appointments" },
                new QuickReplyDto { Label = "➕ Book Appointment", Value = "Book new appointment" }
            };

            return new AgentSummaryResponseDto { SummaryMessage = message, QuickReplies = quickReplies };
        }

        #endregion

    }
}

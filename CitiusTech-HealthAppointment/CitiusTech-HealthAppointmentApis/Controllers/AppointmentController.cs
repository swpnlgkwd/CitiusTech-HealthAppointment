using CitiusTech_HealthAppointmentApis.Dto;
using Microsoft.AspNetCore.Mvc;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Business.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CitiusTech_HealthAppointmentApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAppointmentManager _appointmentManager;
        private readonly IAuthManager _authManager;

        public AppointmentController(ILogger<AppointmentController> logger, IAppointmentManager appointmentManager, IAuthManager authManager, IProviderManager providerManager)
        {
            _logger = logger;
            _appointmentManager = appointmentManager;
            _authManager = authManager;
        }

        [HttpGet]
        public async Task<IEnumerable<AppointmentInfoDto>> Get()
        {
            var user = (await _authManager.GetLoggedInUserInfo());
            if (user == null)
            {
                _logger.LogError("User ID is null. Cannot fetch appointments.");
                return Enumerable.Empty<AppointmentInfoDto>();
            }
            return await _appointmentManager.FetchAppointmentAsync(user.userId, user.userRole);
        }
    }
}

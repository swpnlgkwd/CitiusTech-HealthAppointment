using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientAppointments.Business.Services;

namespace CitiusTech_HealthAppointmentApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GreetingController : ControllerBase
    {
        private readonly GreetingService _greetingService;

        public GreetingController(GreetingService greetingService)
        {
            _greetingService = greetingService;
        }

        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailySummaryAsync()
        {
            var message = await _greetingService.GetDailySummaryAsync();
            return Ok( message );
        }
    }
}

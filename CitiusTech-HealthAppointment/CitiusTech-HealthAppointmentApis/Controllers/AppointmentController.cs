using CitiusTech_HealthAppointmentApis.Agent.Handlers.AppointmentBooking;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Controllers
{
    [ApiController]
    [Route("api/appointment")]
    public class AppointmentController : ControllerBase
    {
        private readonly ResolveAppointmentBookingToolHandler _handler;

        public AppointmentController(ResolveAppointmentBookingToolHandler handler)
        {
            _handler = handler;
        }

        // [HttpPost("book")]
        // public async Task<IActionResult> Book([FromBody] JsonElement payload)
        // {
        //     // var output = await _handler.HandleAsync(null, payload);
        //     return Ok(output);
        // }
    }
}
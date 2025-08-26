using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Dtos;
using PatientAppointments.Core.Contracts;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentManager _appointmentManager;

        public AppointmentsController(IAppointmentManager appointmentManager)
        {
            _appointmentManager = appointmentManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAppointmentDto dto)
        {
            var result = await _appointmentManager.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctor(Guid doctorId)
        {
            var result = await _appointmentManager.GetByDoctorAsync(doctorId);
            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(Guid patientId)
        {
            var result = await _appointmentManager.GetByPatientAsync(patientId);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateAppointmentDto dto)
        {
            var result = await _appointmentManager.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var success = await _appointmentManager.CancelAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.Appointment;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Business.Services;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.Appointment
{
    public class FetchAppointmentByPatientToolHander : BaseToolHandler
    {
        private readonly IAppointmentManager _appointmentsManager;
        public FetchAppointmentByPatientToolHander(
            ILogger<FetchAppointmentByPatientToolHander> logger,
            IAppointmentManager appointmentManager) : base(logger)
        {
            _appointmentsManager = appointmentManager;
        }

        public override string ToolName => FetchAppointmentByPatientTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try
            {
                // 🔍 Parse the filters
                int? patientId = root.FetchInt("patientId");
                int? appointmentStatus = root.FetchInt("appointmentStatusId");
                DateTime? sDate = root.FetchDateTime("startDate");
                DateTime? eDate = root.FetchDateTime("endDate");

                if (patientId == null)
                {
                    throw new Exception("PatientId cannot be null");
                }

                _logger.LogInformation("Fetching the appointments for given patient");

                var appointments = await _appointmentsManager.GetByPatientAsync((int)patientId);
                var filteredResult = appointments;
                if (appointmentStatus != null) appointments.Where(x => x.StatusId == appointmentStatus).ToList();
                if (sDate != null) appointments.Where(x=>x.StartUtc >= sDate).ToList();
                if (eDate != null) appointments.Where(x=>x.EndUtc <= eDate).ToList();

                var response = new
                {
                    success = true,
                    message = "😊 Appointments fetched successfully!",
                    data = filteredResult
                };

                return CreateSuccess(call.Id, "Appointment types fetched successfully", response);
            }
            catch
            {
                _logger.LogInformation("Failed fetching the appointments");
                throw new Exception("Unable to retrieve the appointments");
            }
        }
    }
}

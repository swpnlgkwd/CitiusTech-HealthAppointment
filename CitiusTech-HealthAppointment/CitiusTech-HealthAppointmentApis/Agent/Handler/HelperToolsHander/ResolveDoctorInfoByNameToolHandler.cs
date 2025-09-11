using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Tools.HelperTools;
using CitiusTech_HealthAppointmentApis.Common;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Core.Entities;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CitiusTech_HealthAppointmentApis.Agent.Handler.HelperToolsHander
{
    /// <summary>
    /// Handler to resolve natural language date(s) into a standardized date range (yyyy-MM-dd).
    /// Supports single date or a range in any order, with or without years.
    /// </summary>
    public class ResolveDoctorInfoByNameToolHandler : BaseToolHandler
    {
        // Inject
        private readonly IProviderManager _providerManager;

        public ResolveDoctorInfoByNameToolHandler(ILogger<ResolveNaturalLanguageDateToolHandler> _logger, IProviderManager providerManager)
            : base(_logger) // ✅ common logging/error helpers
        {
            _providerManager = providerManager;
        }

        public override string ToolName => ResolveDoctorInfoByNameTool.GetTool().Name;

        public override async Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root)
        {
            try { 
                // Extract parameters
                var name = root.GetProperty("name").GetString() ?? throw new ArgumentException("Missing 'name' parameter");
                // Validate parameters
                if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || !Regex.IsMatch(name, @"^[a-zA-Z\s'-]+$"))
                {
                    throw new ArgumentException("Invalid 'name' parameter. Must be at least 2 characters and contain only letters, spaces, hyphens, or apostrophes.");
                }

                //remove Dr. or Dr from name if exists
                name = Regex.Replace(name, @"\bDr\.?\s*", "", RegexOptions.IgnoreCase).Trim();

                // Call business logic to get doctor info by name
                var doctors = await _providerManager.GetAllAsync();
                
                var result = doctors.FirstOrDefault(d => d.FullName.Contains(name, StringComparison.OrdinalIgnoreCase) && d.IsActive);
                if (result == null)
                {
                    return CreateError(call.Id, $"No active doctor found matching the name '{name}'.");
                }
                // Return success response with doctor info
                _logger.LogInformation("Doctor found: {DoctorName}", result.FullName);
                return CreateSuccess(call.Id, "Doctor found.", result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {ToolName} with input: {Input}", ToolName, root.ToString());
                throw; // Let the base class handle the error response
            }
        }
    }
}

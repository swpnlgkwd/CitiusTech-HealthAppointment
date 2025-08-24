using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CitiusTech_HealthAppointmentApis.Common.Handlers
{
    public abstract class BaseToolHandler
    {
        protected readonly ILogger _logger;

        protected BaseToolHandler(ILogger logger)
        {
            _logger = logger;
        }

        public abstract string ToolName { get; }
        public abstract Task<ToolOutput?> HandleAsync(RequiredFunctionToolCall call, JsonElement root);

        protected ToolOutput CreateSuccess(string? id, string message, object result)
            => new ToolOutput { Id = id, Success = true, Message = message, Result = result };

        protected ToolOutput CreateError(string? id, string message)
            => new ToolOutput { Id = id, Success = false, Message = message };
    }

    public class ToolOutput
    {
        public string? Id { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Result { get; set; }
    }

    public class RequiredFunctionToolCall
    {
        public string Id { get; set; }
    }
}
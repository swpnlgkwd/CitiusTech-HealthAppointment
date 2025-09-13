using CitiusTech_HealthAppointmentApis.Agent.Handler;

namespace CitiusTech_HealthAppointmentApis.Agent.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddToolHandlers(this IServiceCollection services)
        {
            var toolHandlerType = typeof(IToolHandler);
            var handlers = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => toolHandlerType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                        .ToList();

            foreach (var handler in handlers)
            {
                services.AddScoped(toolHandlerType, handler);
            }
        }
    }
}

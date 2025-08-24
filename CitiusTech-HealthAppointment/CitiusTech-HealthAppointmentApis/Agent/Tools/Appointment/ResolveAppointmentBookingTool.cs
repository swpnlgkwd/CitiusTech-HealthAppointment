using Azure.AI.Agents.Persistent;

namespace CitiusTech_HealthAppointmentApis.Agent.Tools.AppointmentBooking
{
    public static class ResolveAppointmentBookingTool
    {
        public static FunctionToolDefinition GetTool()
        {
            return new FunctionToolDefinition(
                name: "bookAppointment",
                description: "Books an appointment for a patient with a provider.",
                parameters: new[]
                {
                    new FunctionToolParameter("patientId", FunctionToolParameterType.Integer, "Patient ID", isRequired: true),
                    new FunctionToolParameter("providerId", FunctionToolParameterType.Integer, "Provider ID", isRequired: true),
                    new FunctionToolParameter("slotId", FunctionToolParameterType.Integer, "Slot ID", isRequired: true),
                    new FunctionToolParameter("statusId", FunctionToolParameterType.Integer, "Status ID", isRequired: true),
                    new FunctionToolParameter("typeId", FunctionToolParameterType.Integer, "Type ID", isRequired: true),
                    new FunctionToolParameter("notes", FunctionToolParameterType.String, "Notes", isRequired: false)
                }
            );
        }
    }
}
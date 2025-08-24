using System;

namespace CitiusTech_HealthAppointmentApis.Common.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}
using PatientAppointments.Business.Dtos;

namespace PatientAppointments.Business.Contracts.Risk
{
    public interface IPatientRiskManager
    {
        Task<PatientRiskDto?> GetPatientWithRisksAsync(int patientId);
    }
}
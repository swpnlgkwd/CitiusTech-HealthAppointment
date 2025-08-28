using PatientAppointments.Core.Contracts.Repositories.Base;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Core.Contracts.Repositories
{
    public interface IPatientRepository : IGenericRepository<Patient> { }
}

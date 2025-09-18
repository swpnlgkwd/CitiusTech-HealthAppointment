using PatientAppointments.Core.Entities;
using PatientAppointments.Core.Entities.Risk;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Contracts.Risk
{
    public interface IRiskTypeManager
    {
        Task<IEnumerable<RiskType>> GetAllAsync();
    }
}

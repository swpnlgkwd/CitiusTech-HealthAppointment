using PatientAppointments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Contracts
{
    public interface IProviderManager
    {
        Task<IEnumerable<Provider>> GetAllAsync();
    }
}

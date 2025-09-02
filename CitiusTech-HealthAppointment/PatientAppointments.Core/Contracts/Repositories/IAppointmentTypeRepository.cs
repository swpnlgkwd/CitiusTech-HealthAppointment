using PatientAppointments.Core.Contracts.Repositories.Base;
using PatientAppointments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Core.Contracts.Repositories
{
    public interface IAppointmentTypeRepository : IGenericRepository<AppointmentType>
    {
    }
}

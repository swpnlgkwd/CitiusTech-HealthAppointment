using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Contracts
{
    public interface IDoctorInfoManager
    {
        Task<object> FetchDoctorInfoBySpeciality(int specialityId);


        Task<IEnumerable<object>> FetchDoctorInfoByName(string name);
    }
}

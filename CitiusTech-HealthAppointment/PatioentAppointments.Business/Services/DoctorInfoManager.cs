using Microsoft.EntityFrameworkCore;
using PatientAppointments.Business.Contracts;
using PatientAppointments.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Services
{
    public class DoctorInfoManager : IDoctorInfoManager
    {
        private readonly IUnitOfWork _uow;

        public DoctorInfoManager(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<object> FetchDoctorInfoBySpeciality(int specialityId)
        {

            return await _uow.Provider.GetAllAsync();
        }


        public async Task<IEnumerable<object>> FetchDoctorInfoByName(string name)
        {

            var doctors = await _uow.Provider.FindAsync(
                     s => EF.Functions.Like(s.FullName, $"%{name}%")
                     );

            return doctors;
        }
    }
}

using PatientAppointments.Business.Contracts;
using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Services
{
    public class SpecialtyManager: ISpecialtyManager
    {
        private readonly ISpecialityRepository specialityRepository;
        public SpecialtyManager(ISpecialityRepository specialityRepository) {
            this.specialityRepository = specialityRepository;
        }

        public Task<IEnumerable<Specialty>> GetAllAsync()
        {
            return specialityRepository.GetAllAsync();
        }
    }
}

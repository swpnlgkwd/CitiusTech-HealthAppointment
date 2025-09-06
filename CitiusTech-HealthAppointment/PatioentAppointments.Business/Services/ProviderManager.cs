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
    public class ProviderManager: IProviderManager
    {
        private readonly IProviderRepository _providerRepository;

        //constructor
        public ProviderManager(IProviderRepository providerRepository)
        {
            _providerRepository = providerRepository;
        }

        public Task<IEnumerable<Provider>> GetAllAsync()
        {
            return _providerRepository.GetAllAsync();
        }
    }
}

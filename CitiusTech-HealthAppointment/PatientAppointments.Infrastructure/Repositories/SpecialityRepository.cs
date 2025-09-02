using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Data;
using PatientAppointments.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Infrastructure.Repositories
{
    public class SpecialityRepository : ReadOnlyRepositoryBase<Specialty>, ISpecialityRepository
    {
        public SpecialityRepository(AppDbContext ctx) : base(ctx) { }
    }
}

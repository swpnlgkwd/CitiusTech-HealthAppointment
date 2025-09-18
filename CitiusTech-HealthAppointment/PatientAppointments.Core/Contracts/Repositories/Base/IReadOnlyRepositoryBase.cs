using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Core.Contracts.Repositories.Base
{
    public interface IReadOnlyRepositoryBase<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        // Add this
        IQueryable<T> Query();
    }
}

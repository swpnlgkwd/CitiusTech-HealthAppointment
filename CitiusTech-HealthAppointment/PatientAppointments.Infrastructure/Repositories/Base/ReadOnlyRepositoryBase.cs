using Microsoft.EntityFrameworkCore;
using PatientAppointments.Core.Contracts.Repositories;
using PatientAppointments.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Infrastructure.Repositories.Base
{
    public class ReadOnlyRepositoryBase<T> where T : class
    {

        protected readonly AppDbContext _ctx;
        protected readonly DbSet<T> _db;


        public ReadOnlyRepositoryBase(AppDbContext ctx) { _ctx = ctx; _db = ctx.Set<T>(); }

        public async Task<IEnumerable<T>> GetAllAsync() => await _db.ToListAsync();
    }
}

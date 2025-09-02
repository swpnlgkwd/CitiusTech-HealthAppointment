using Microsoft.EntityFrameworkCore;
using PatientAppointments.Core.Contracts.Repositories.Base;
using PatientAppointments.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PatientAppointments.Infrastructure.Repositories.Base
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _ctx;
        protected readonly DbSet<T> _db;
        public GenericRepository(AppDbContext ctx) { _ctx = ctx; _db = ctx.Set<T>(); }
        public async Task<T?> GetByIdAsync(int id) => await _db.FindAsync(id);
        public async Task<IEnumerable<T>> GetAllAsync() => await _db.ToListAsync();
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _db.Where(predicate).ToListAsync();
        public async Task AddAsync(T entity) { await _db.AddAsync(entity); }
        public void Update(T entity) => _db.Update(entity);
        public void Remove(T entity) => _db.Remove(entity);

        // Expose IQueryable
        public IQueryable<T> Query() => _db.AsQueryable();
    }
}

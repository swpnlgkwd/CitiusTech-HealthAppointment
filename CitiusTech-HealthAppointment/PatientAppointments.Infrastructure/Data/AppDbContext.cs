using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PatientAppointments.Core.Entities;
using PatientAppointments.Infrastructure.Identity;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PatientAppointments.Infrastructure.Data {
    public class AppDbContext : IdentityDbContext<ApplicationUser> {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; } = default!;
        public DbSet<Doctor> Doctors { get; set; } = default!;
        public DbSet<Appointment> Appointments { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var e in entries) {
                switch (e.State) {
                    case EntityState.Added:
                        e.Entity.CreatedAt = DateTime.UtcNow;
                        e.Entity.UpdatedAt = null;
                        break;
                    case EntityState.Modified:
                        e.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        e.State = EntityState.Modified;
                        e.Entity.IsDeleted = true;
                        e.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

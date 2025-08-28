using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Infrastructure.Data.Configurations {
    public class PatientConfiguration : BaseEntityConfiguration<Patient> {
        public override void Configure(EntityTypeBuilder<Patient> builder) {
            base.Configure(builder);
            builder.ToTable("Patients");
            builder.HasKey(p => p.PatientId);
            builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Phone).HasMaxLength(20);
            builder.Property(p => p.Gender).HasMaxLength(10);
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}

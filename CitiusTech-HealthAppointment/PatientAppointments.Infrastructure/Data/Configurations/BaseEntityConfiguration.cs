using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Infrastructure.Data.Configurations {
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity {
        public virtual void Configure(EntityTypeBuilder<T> builder) {
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired(false);
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Infrastructure.Data.Configurations {
    public class PatientConfiguration : BaseEntityConfiguration<Patient> {
        public override void Configure(EntityTypeBuilder<Patient> builder) {
            base.Configure(builder);
            // Map to table
            builder.ToTable("Patient");

            // Primary key
            builder.HasKey(p => p.PatientId);

            // Properties
            builder.Property(p => p.PatientId)
                .HasColumnName("patient_id") // Matches the schema column name
                .ValueGeneratedOnAdd();

            builder.Property(p => p.FullName)
                .HasColumnName("full_name") // Matches the schema column name
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Dob)
                .HasColumnName("dob") // Matches the schema column name
                .IsRequired(false);

            builder.Property(p => p.GenderId)
                .HasColumnName("gender_id") // Matches the schema column name
                .IsRequired(false);

            builder.Property(p => p.Email)
                .HasColumnName("email") // Matches the schema column name
                .HasMaxLength(150);

            builder.Property(p => p.Phone)
                .HasColumnName("phone") // Matches the schema column name
                .HasMaxLength(50);

            builder.Property(p => p.Address)
                .HasColumnName("address") // Matches the schema column name
                .HasMaxLength(250);

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at") // Matches the schema column name
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(p => p.UpdatedAt)
                .HasColumnName("updated_at") // Matches the schema column name
                .IsRequired(false);

            builder.Property(p => p.IsDeleted)
                .HasColumnName("is_deleted") // Matches the schema column name
                .HasDefaultValue(false);
        }
    }
}

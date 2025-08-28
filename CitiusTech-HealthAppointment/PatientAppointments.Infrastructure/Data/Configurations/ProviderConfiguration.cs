using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;


using Microsoft.EntityFrameworkCore;

namespace PatientAppointments.Infrastructure.Data.Configurations
{
    public class ProviderConfiguration : BaseEntityConfiguration<Provider>
    {
        public override void Configure(EntityTypeBuilder<Provider> builder)
        {
            base.Configure(builder);
            // Map to table
            builder.ToTable("Provider");

            // Primary key
            builder.HasKey(p => p.ProviderId);

            // Properties
            builder.Property(p => p.ProviderId)
                .HasColumnName("provider_id") // Matches the schema column name
                .ValueGeneratedOnAdd();

            builder.Property(p => p.FullName)
                .HasColumnName("full_name") // Matches the schema column name
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.SpecialtyId)
                .HasColumnName("specialty_id") // Matches the schema column name
                .IsRequired(false);

            builder.Property(p => p.ContactEmail)
                .HasColumnName("contact_email") // Matches the schema column name
                .HasMaxLength(150);

            builder.Property(p => p.ContactPhone)
                .HasColumnName("contact_phone") // Matches the schema column name
                .HasMaxLength(50);

            builder.Property(p => p.IsActive)
                .HasColumnName("is_active") // Matches the schema column name
                .HasDefaultValue(true);
        }
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Infrastructure.Data.Configurations
{
    public class SpecialtyConfiguration : BaseEntityConfiguration<Specialty>
    {
        public override void Configure(EntityTypeBuilder<Specialty> builder)
        {
            base.Configure(builder);
            // Map to table
            builder.ToTable("Specialty");

            // Primary key
            builder.HasKey(ps => ps.SpecialityId);

            // Properties
            builder.Property(ps => ps.SpecialityId)
                .HasColumnName("specialty_id")
                .ValueGeneratedOnAdd();

            builder.Property(ps => ps.SpecialtyName)
                .HasColumnName("specialty_name");
        }
    }
}

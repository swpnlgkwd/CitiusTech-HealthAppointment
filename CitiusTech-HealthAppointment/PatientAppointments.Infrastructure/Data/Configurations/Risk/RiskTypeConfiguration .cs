using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities.Risk;

namespace PatientAppointments.Infrastructure.Data.Configurations.Risk
{
    public class RiskTypeConfiguration : IEntityTypeConfiguration<RiskType>
    {
        public void Configure(EntityTypeBuilder<RiskType> builder)
        {
            // Map to table
            builder.ToTable("RiskType");

            // Primary key
            builder.HasKey(rt => rt.RiskTypeId);

            // Properties
            builder.Property(rt => rt.RiskTypeId)
                .HasColumnName("risk_type_id");

            builder.Property(rt => rt.RiskTypeName)
                .HasColumnName("risk_type_name")
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}

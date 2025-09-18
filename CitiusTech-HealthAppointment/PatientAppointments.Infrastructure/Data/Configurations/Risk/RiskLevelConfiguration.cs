using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities.Risk;

namespace PatientAppointments.Infrastructure.Data.Configurations.Risk
{
    public class RiskLevelConfiguration : IEntityTypeConfiguration<RiskLevel>
    {
        public void Configure(EntityTypeBuilder<RiskLevel> builder)
        {
            // Map to table
            builder.ToTable("RiskLevel");

            // Primary key
            builder.HasKey(rl => rl.RiskLevelId);

            // Properties
            builder.Property(rl => rl.RiskLevelId)
                .HasColumnName("risk_level_id");

            builder.Property(rl => rl.RiskLevelName)
                .HasColumnName("risk_level_name")
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}

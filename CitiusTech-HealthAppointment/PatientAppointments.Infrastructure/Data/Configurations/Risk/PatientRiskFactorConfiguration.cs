using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities.Risk;

namespace PatientAppointments.Infrastructure.Data.Configurations.Risk
{
    public class PatientRiskFactorConfiguration : BaseEntityConfiguration<PatientRiskFactor>
    {
        public override void Configure(EntityTypeBuilder<PatientRiskFactor> builder)
        {
            base.Configure(builder);

            // Map to table
            builder.ToTable("PatientRiskFactor");

            // Primary key
            builder.HasKey(pf => pf.RiskId);

            // Properties
            builder.Property(pf => pf.RiskId)
                .HasColumnName("risk_id")
                .ValueGeneratedOnAdd();

            builder.Property(pf => pf.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            builder.Property(pf => pf.RiskTypeId)
                .HasColumnName("risk_type_id")
                .IsRequired();

            builder.Property(pf => pf.RiskLevelId)
                .HasColumnName("risk_level_id")
                .IsRequired();

            builder.Property(pf => pf.IdentifiedOn)
                .HasColumnName("identified_on")
                .HasColumnType("date")
                .HasDefaultValueSql("CAST(SYSUTCDATETIME() AS DATE)");
        }
    }
}

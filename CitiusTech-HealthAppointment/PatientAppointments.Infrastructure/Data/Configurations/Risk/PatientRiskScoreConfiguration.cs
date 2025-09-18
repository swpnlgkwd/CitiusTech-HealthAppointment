using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities.Risk;

namespace PatientAppointments.Infrastructure.Data.Configurations.Risk
{
    public class PatientRiskScoreConfiguration : IEntityTypeConfiguration<PatientRiskScore>
    {
        public void Configure(EntityTypeBuilder<PatientRiskScore> builder)
        {
            // Table mapping
            builder.ToTable("PatientRiskScore");

            // Composite primary key
            builder.HasKey(ps => new { ps.PatientId });

            // Columns
            builder.Property(ps => ps.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            builder.Property(ps => ps.Score)
                .HasColumnName("score")
                .IsRequired();

            builder.Property(ps => ps.RiskLevelId)
                .HasColumnName("risk_level_id")
                .IsRequired();

            builder.Property(ps => ps.Reason)
                .HasColumnName("reason")
                .HasColumnType("nvarchar(max)");

            builder.Property(ps => ps.CalculatedAt)
                .HasColumnName("calculated_at")
                .HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Infrastructure.Data.Configurations
{
    public class ProviderScheduleConfiguration : BaseEntityConfiguration<ProviderSchedule>
    {
        public override void Configure(EntityTypeBuilder<ProviderSchedule> builder)
        {
            base.Configure(builder);
            // Map to table
            builder.ToTable("ProviderSchedule");

            // Primary key
            builder.HasKey(ps => ps.ScheduleId);

            // Properties
            builder.Property(ps => ps.ScheduleId)
                .HasColumnName("ScheduleId")
                .ValueGeneratedOnAdd();

            builder.Property(ps => ps.ProviderId)
                .HasColumnName("ProviderId")
                .IsRequired();

            builder.Property(ps => ps.ScheduleDate)
                .HasColumnName("ScheduleDate")
                .IsRequired();

            builder.Property(ps => ps.StartTime)
                .HasColumnName("StartTime")
                .IsRequired();

            builder.Property(ps => ps.EndTime)
                .HasColumnName("EndTime")
                .IsRequired();

            builder.Property(ps => ps.SlotDurationMinutes)
                .HasColumnName("SlotDurationMinutes")
                .HasDefaultValue(60);
        }
    }
}

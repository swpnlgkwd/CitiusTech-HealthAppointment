using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Infrastructure.Data.Configurations
{
    public class ProviderSlotConfiguration : BaseEntityConfiguration<ProviderSlot>
    {
        public override void Configure(EntityTypeBuilder<ProviderSlot> builder)
        {
            base.Configure(builder);
            // Map to table
            builder.ToTable("ProviderSlots");

            // Primary key
            builder.HasKey(ps => ps.SlotId);

            // Properties
            builder.Property(ps => ps.SlotId)
                .HasColumnName("SlotId")
                .ValueGeneratedOnAdd();

            builder.Property(ps => ps.ScheduleId)
                .HasColumnName("ScheduleId")
                .IsRequired();

            builder.Property(ps => ps.SlotStart)
                .HasColumnName("SlotStart")
                .IsRequired();

            builder.Property(ps => ps.SlotEnd)
                .HasColumnName("SlotEnd")
                .IsRequired();

            builder.Property(ps => ps.IsBooked)
                .HasColumnName("IsBooked")
                .HasDefaultValue(false);
        }
    }
}

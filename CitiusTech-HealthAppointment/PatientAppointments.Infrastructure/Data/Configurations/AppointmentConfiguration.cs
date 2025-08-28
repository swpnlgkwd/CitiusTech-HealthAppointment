using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Infrastructure.Data.Configurations {
    public class AppointmentConfiguration : BaseEntityConfiguration<Appointment> {
        public override void Configure(EntityTypeBuilder<Appointment> builder) {
            base.Configure(builder);
            // Map to table
            builder.ToTable("Appointment");

            // Primary key
            builder.HasKey(a => a.AppointmentId);

            // Properties
            builder.Property(a => a.AppointmentId)
                .HasColumnName("appointment_id")
                .ValueGeneratedOnAdd();

            builder.Property(a => a.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            builder.Property(a => a.ProviderId)
                .HasColumnName("provider_id")
                .IsRequired();

            builder.Property(a => a.SlotId)
                .HasColumnName("slot_id")
                .IsRequired();

            builder.Property(a => a.StartUtc)
                .HasColumnName("start_utc")
                .IsRequired();

            builder.Property(a => a.EndUtc)
                .HasColumnName("end_utc")
                .IsRequired();

            builder.Property(a => a.StatusId)
                .HasColumnName("status_id")
                .IsRequired();

            builder.Property(a => a.TypeId)
                .HasColumnName("type_id")
                .IsRequired();

            builder.Property(a => a.Notes)
                .HasColumnName("notes")
                .HasColumnType("NVARCHAR(MAX)");

            builder.Property(a => a.ReminderSent)
                .HasColumnName("reminder_sent")
                .HasDefaultValue(false);

            builder.Property(a => a.ReminderSentAt)
                .HasColumnName("reminder_sent_at")
                .IsRequired(false);

            builder.Property(a => a.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(a => a.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired(false);

            builder.Property(a => a.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}

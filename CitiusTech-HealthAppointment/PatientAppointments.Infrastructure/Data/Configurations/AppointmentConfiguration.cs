using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;

namespace PatientAppointments.Infrastructure.Data.Configurations {
    public class AppointmentConfiguration : BaseEntityConfiguration<Appointment> {
        public override void Configure(EntityTypeBuilder<Appointment> builder) {
            base.Configure(builder);
            builder.ToTable("Appointments");
            builder.HasKey(a => a.AppointmentId);
            builder.Property(a => a.Status).HasMaxLength(20).HasDefaultValue("Scheduled");
            builder.Property(a => a.Notes).HasMaxLength(500);
            builder.HasOne(a => a.Patient).WithMany().HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.Doctor).WithMany().HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Cascade);
            builder.HasQueryFilter(a => !a.IsDeleted);
        }
    }
}

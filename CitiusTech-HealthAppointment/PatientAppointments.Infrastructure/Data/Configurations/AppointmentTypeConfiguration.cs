using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PatientAppointments.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Infrastructure.Data.Configurations
{
    internal class AppointmentTypeConfiguration : BaseEntityConfiguration<AppointmentType>
    {
        public override void Configure(EntityTypeBuilder<AppointmentType> builder)
        {
            base.Configure(builder);
            // Map to table
            builder.ToTable("AppointmentType");

            // Primary key
            builder.HasKey(ps => ps.type_id);

            // Properties
            builder.Property(ps => ps.type_name)
                .HasColumnName("type_name");
        }
    
    }
}

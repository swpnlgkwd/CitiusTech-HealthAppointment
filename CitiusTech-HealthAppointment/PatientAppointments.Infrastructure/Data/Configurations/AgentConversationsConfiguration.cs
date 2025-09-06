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
    public class AgentConversationsConfiguration : IEntityTypeConfiguration<AgentConversations>
    {
        public void Configure(EntityTypeBuilder<AgentConversations> builder)
        {
            // Map to table
            builder.ToTable("AgentConversations");

            // Primary key
            builder.HasKey(ps => ps.user_id);

            // Properties
            builder.Property(ps => ps.user_id)
                .HasColumnName("user_id");

            builder.Property(ps => ps.thread_id)
                .HasColumnName("thread_id");

            builder.Property(ps => ps.created_at)
                .HasColumnName("created_at");
        }
    }
}

using System;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class XrefServiceScheduleServiceTaskConfiguration : IEntityTypeConfiguration<XrefServiceScheduleServiceTask>
{
    public void Configure(EntityTypeBuilder<XrefServiceScheduleServiceTask> builder)
    {
        builder.ToTable("XrefServiceScheduleServiceTasks");
        builder.HasKey(ssst => new { ssst.ServiceScheduleID, ssst.ServiceTaskID });

        // Regular Indexes
        builder.HasIndex(ssst => ssst.ServiceScheduleID);
        builder.HasIndex(ssst => ssst.ServiceTaskID);

        // Unique constraint to prevent duplicate tasks in same schedule
        builder.HasIndex(ssst => new { ssst.ServiceScheduleID, ssst.ServiceTaskID }).IsUnique();

        // Table Relationships
        builder
            .HasOne(ssst => ssst.ServiceSchedule)
            .WithMany(ss => ss.XrefServiceScheduleServiceTasks)
            .HasForeignKey(ssst => ssst.ServiceScheduleID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ssst => ssst.ServiceTask)
            .WithMany(ss => ss.XrefServiceScheduleServiceTasks)
            .HasForeignKey(ssst => ssst.ServiceTaskID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
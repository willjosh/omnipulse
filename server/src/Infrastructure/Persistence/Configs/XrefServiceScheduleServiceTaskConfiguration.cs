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
        builder.HasKey(xssst => new { xssst.ServiceScheduleID, xssst.ServiceTaskID });

        // Regular Indexes
        builder.HasIndex(xssst => xssst.ServiceScheduleID);
        builder.HasIndex(xssst => xssst.ServiceTaskID);

        // Unique constraint to prevent duplicate tasks in same schedule
        builder.HasIndex(xssst => new { xssst.ServiceScheduleID, xssst.ServiceTaskID }).IsUnique();

        // Table Relationships
        // XrefServiceScheduleServiceTask N:1 ServiceSchedule
        builder
            .HasOne(xssst => xssst.ServiceSchedule)
            .WithMany(ss => ss.XrefServiceScheduleServiceTasks)
            .HasForeignKey(xssst => xssst.ServiceScheduleID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(xssst => xssst.ServiceTask)
            .WithMany(ss => ss.XrefServiceScheduleServiceTasks)
            .HasForeignKey(xssst => xssst.ServiceTaskID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
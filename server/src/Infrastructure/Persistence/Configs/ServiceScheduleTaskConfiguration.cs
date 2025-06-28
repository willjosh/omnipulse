using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class ServiceScheduleTaskConfiguration : IEntityTypeConfiguration<ServiceScheduleTask>
{
    public void Configure(EntityTypeBuilder<ServiceScheduleTask> builder)
    {
        builder.ToTable("ServiceScheduleTasks");
        builder.HasKey(sst => sst.ID);

        // Regular Indexes
        builder.HasIndex(sst => sst.ServiceScheduleID);
        builder.HasIndex(sst => sst.ServiceTaskID);
        builder.HasIndex(sst => sst.IsMandatory);

        // Composite indexes for common queries
        builder.HasIndex(sst => new { sst.ServiceScheduleID, sst.SequenceNumber });
        builder.HasIndex(sst => new { sst.ServiceScheduleID, sst.IsMandatory });

        // Unique constraint to prevent duplicate tasks in same schedule
        builder.HasIndex(sst => new { sst.ServiceScheduleID, sst.ServiceTaskID }).IsUnique();

        // Unique constraint for sequence numbers within a schedule
        builder.HasIndex(sst => new { sst.ServiceScheduleID, sst.SequenceNumber }).IsUnique();

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceScheduleTask_SequenceNumber",
            "SequenceNumber > 0"));

        // Table Relationships
        builder
            .HasOne(sst => sst.ServiceSchedule)
            .WithMany(ss => ss.ServiceScheduleTasks)
            .HasForeignKey(sst => sst.ServiceScheduleID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(sst => sst.ServiceTask)
            .WithMany()
            .HasForeignKey(sst => sst.ServiceTaskID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
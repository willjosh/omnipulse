using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class ServiceScheduleConfiguration : IEntityTypeConfiguration<ServiceSchedule>
{
    public void Configure(EntityTypeBuilder<ServiceSchedule> builder)
    {
        builder.ToTable("ServiceSchedules");
        builder.HasKey(ss => ss.ID);

        // String Length Constraints
        builder.Property(ss => ss.Name).HasMaxLength(200);

        // Regular Indexes
        builder.HasIndex(ss => ss.ServiceProgramID);
        builder.HasIndex(ss => ss.IsActive);
        builder.HasIndex(ss => ss.Name);

        // Composite indexes for common queries
        builder.HasIndex(ss => new { ss.ServiceProgramID, ss.IsActive });
        builder.HasIndex(ss => new { ss.IsActive, ss.Name });

        // Unique constraint within service program
        builder.HasIndex(ss => new { ss.ServiceProgramID, ss.Name }).IsUnique();

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_IntervalMileage",
            "IntervalMileage > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_IntervalDays",
            "IntervalDays > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_IntervalHours",
            "IntervalHours >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_BufferMileage",
            "BufferMileage >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_BufferDays",
            "BufferDays >= 0"));


        // Table Relationships
        builder
            .HasOne(ss => ss.ServiceProgram)
            .WithMany(sp => sp.ServiceSchedules)
            .HasForeignKey(ss => ss.ServiceProgramID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(ss => ss.ServiceScheduleTasks)
            .WithOne(sst => sst.ServiceSchedule)
            .HasForeignKey(sst => sst.ServiceScheduleID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
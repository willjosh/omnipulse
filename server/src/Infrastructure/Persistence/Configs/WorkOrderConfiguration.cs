using System;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable("WorkOrders");
        builder.HasKey(wo => wo.ID);

        // String Length Constraints
        builder.Property(wo => wo.Title).HasMaxLength(200);
        builder.Property(wo => wo.Description).HasMaxLength(2000);

        // Regular Indexes
        builder.HasIndex(wo => wo.VehicleID);
        builder.HasIndex(wo => wo.ServiceReminderID);
        builder.HasIndex(wo => wo.AssignedToUserID);
        builder.HasIndex(wo => wo.Status);
        builder.HasIndex(wo => wo.PriorityLevel);
        builder.HasIndex(wo => wo.WorkOrderType);
        builder.HasIndex(wo => wo.ScheduledStartDate);
        builder.HasIndex(wo => wo.ActualStartDate);
        builder.HasIndex(wo => wo.CreatedAt);

        // Composite indexes for common queries
        builder.HasIndex(wo => new { wo.VehicleID, wo.Status });
        builder.HasIndex(wo => new { wo.AssignedToUserID, wo.Status });
        builder.HasIndex(wo => new { wo.Status, wo.PriorityLevel });
        builder.HasIndex(wo => new { wo.Status, wo.ScheduledStartDate });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_EstimatedCost",
            "EstimatedCost IS NULL OR EstimatedCost >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_ActualCost",
            "ActualCost IS NULL OR ActualCost >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_ActualHours",
            "ActualHours IS NULL OR ActualHours >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_StartOdometer",
            "StartOdometer >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_EndOdometer",
            "EndOdometer IS NULL OR EndOdometer >= StartOdometer"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_Dates",
            "ActualStartDate IS NULL OR ScheduledStartDate IS NULL OR ActualStartDate >= ScheduledStartDate"));

        // Table Relationships
        builder
            .HasOne(wo => wo.Vehicle)
            .WithMany()
            .HasForeignKey(wo => wo.VehicleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(wo => wo.ServiceReminder)
            .WithMany()
            .HasForeignKey(wo => wo.ServiceReminderID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(wo => wo.User)
            .WithMany()
            .HasForeignKey(wo => wo.AssignedToUserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
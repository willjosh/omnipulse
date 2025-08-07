using Domain.Entities;
using Domain.Entities.Enums;

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

        builder.HasIndex(wo => wo.VehicleID)
            .HasDatabaseName("IX_WorkOrders_VehicleID");
        builder.HasIndex(wo => wo.AssignedToUserID)
            .HasDatabaseName("IX_WorkOrders_AssignedToUserID");
        builder.HasIndex(wo => wo.Status)
            .HasDatabaseName("IX_WorkOrders_Status");
        builder.HasIndex(wo => wo.PriorityLevel)
            .HasDatabaseName("IX_WorkOrders_PriorityLevel");
        builder.HasIndex(wo => wo.WorkOrderType)
            .HasDatabaseName("IX_WorkOrders_WorkOrderType");
        builder.HasIndex(wo => wo.ScheduledStartDate)
            .HasDatabaseName("IX_WorkOrders_ScheduledStartDate");
        builder.HasIndex(wo => wo.ActualStartDate)
            .HasDatabaseName("IX_WorkOrders_ActualStartDate");
        builder.HasIndex(wo => wo.ScheduledCompletionDate)
            .HasDatabaseName("IX_WorkOrders_ScheduledCompletionDate");
        builder.HasIndex(wo => wo.ActualCompletionDate)
            .HasDatabaseName("IX_WorkOrders_ActualCompletionDate");
        builder.HasIndex(wo => wo.CreatedAt)
            .HasDatabaseName("IX_WorkOrders_CreatedAt");

        builder.HasIndex(wo => wo.Status)
            .HasFilter($"{nameof(WorkOrder.Status)} = {(int)WorkOrderStatusEnum.CREATED}")
            .HasDatabaseName("IX_WorkOrders_Status_Created");

        builder.HasIndex(wo => wo.Status)
            .HasFilter($"{nameof(WorkOrder.Status)} = {(int)WorkOrderStatusEnum.IN_PROGRESS}")
            .HasDatabaseName("IX_WorkOrders_Status_InProgress");

        builder.HasIndex(wo => wo.Status)
            .HasFilter($"{nameof(WorkOrder.Status)} = {(int)WorkOrderStatusEnum.COMPLETED}")
            .HasDatabaseName("IX_WorkOrders_Status_Completed");

        builder.HasIndex(wo => wo.Status)
            .HasFilter($"{nameof(WorkOrder.Status)} = {(int)WorkOrderStatusEnum.CANCELLED}")
            .HasDatabaseName("IX_WorkOrders_Status_Cancelled");

        builder.HasIndex(wo => new { wo.VehicleID, wo.Status })
            .HasDatabaseName("IX_WorkOrders_VehicleStatus");

        builder.HasIndex(wo => new { wo.AssignedToUserID, wo.Status })
            .HasDatabaseName("IX_WorkOrders_UserStatus");

        builder.HasIndex(wo => new { wo.Status, wo.PriorityLevel })
            .HasDatabaseName("IX_WorkOrders_StatusPriority");

        builder.HasIndex(wo => new { wo.Status, wo.ScheduledStartDate })
            .HasDatabaseName("IX_WorkOrders_StatusScheduledStart");

        builder.HasIndex(wo => new { wo.Status, wo.CreatedAt })
            .HasDatabaseName("IX_WorkOrders_StatusCreatedAt");

        builder.HasIndex(wo => new { wo.WorkOrderType, wo.Status })
            .HasDatabaseName("IX_WorkOrders_TypeStatus");

        builder.HasIndex(wo => new { wo.PriorityLevel, wo.Status, wo.ScheduledStartDate })
            .HasDatabaseName("IX_WorkOrders_PriorityStatusScheduled");

        builder.HasIndex(wo => new { wo.Status, wo.CreatedAt })
            .IncludeProperties(wo => new
            {
                wo.Title,
                wo.VehicleID,
                wo.AssignedToUserID,
                wo.PriorityLevel,
                wo.WorkOrderType,
                wo.ScheduledStartDate,
                wo.StartOdometer
            })
            .HasDatabaseName("IX_WorkOrders_StatusCreatedCovering");

        builder.HasIndex(wo => new { wo.VehicleID, wo.Status })
            .IncludeProperties(wo => new
            {
                wo.Title,
                wo.PriorityLevel,
                wo.WorkOrderType,
                wo.ScheduledStartDate,
                wo.CreatedAt
            })
            .HasDatabaseName("IX_WorkOrders_VehicleStatusCovering");

        builder.HasIndex(wo => new { wo.Title, wo.Status })
            .HasDatabaseName("IX_WorkOrders_TitleStatus");

        builder.HasIndex(wo => new { wo.ScheduledStartDate, wo.Status })
            .IncludeProperties(wo => new { wo.PriorityLevel, wo.WorkOrderType })
            .HasDatabaseName("IX_WorkOrders_ScheduledStartStatusCovering");

        builder.HasIndex(wo => new { wo.ActualStartDate, wo.Status })
            .HasDatabaseName("IX_WorkOrders_ActualStartStatus");

        builder.HasIndex(wo => new { wo.ScheduledCompletionDate, wo.Status })
            .HasDatabaseName("IX_WorkOrders_ScheduledCompletionStatus");

        builder.HasIndex(wo => new { wo.ActualCompletionDate, wo.Status })
            .HasDatabaseName("IX_WorkOrders_ActualCompletionStatus");

        builder.HasIndex(wo => new { wo.StartOdometer, wo.Status })
            .HasDatabaseName("IX_WorkOrders_StartOdometerStatus");

        builder.HasIndex(wo => new { wo.EndOdometer, wo.Status })
            .HasDatabaseName("IX_WorkOrders_EndOdometerStatus");

        builder.HasIndex(wo => new { wo.AssignedToUserID, wo.ScheduledStartDate })
            .HasFilter($"{nameof(WorkOrder.Status)} IN ({(int)WorkOrderStatusEnum.CREATED}, {(int)WorkOrderStatusEnum.IN_PROGRESS})")
            .HasDatabaseName("IX_WorkOrders_UserScheduledActive");

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_StartOdometer",
            "StartOdometer >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_EndOdometer",
            "EndOdometer IS NULL OR EndOdometer >= StartOdometer"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_ScheduledDates",
            "ScheduledCompletionDate IS NULL OR ScheduledStartDate IS NULL OR ScheduledCompletionDate >= ScheduledStartDate"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrder_ActualDates",
            "ActualCompletionDate IS NULL OR ActualStartDate IS NULL OR ActualCompletionDate >= ActualStartDate"));

        // Table Relationships
        builder
            .HasOne(wo => wo.Vehicle)
            .WithMany()
            .HasForeignKey(wo => wo.VehicleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(wo => wo.User)
            .WithMany()
            .HasForeignKey(wo => wo.AssignedToUserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class WorkOrderLineItemConfiguration : IEntityTypeConfiguration<WorkOrderLineItem>
{
    public void Configure(EntityTypeBuilder<WorkOrderLineItem> builder)
    {
        builder.ToTable("WorkOrderLineItems");
        builder.HasKey(woli => woli.ID);

        // String Length Constraints
        builder.Property(woli => woli.Description).HasMaxLength(500);

        // Precision for decimal fields
        builder.Property(woli => woli.UnitPrice).HasPrecision(10, 2);
        builder.Property(woli => woli.HourlyRate).HasPrecision(10, 2);
        builder.Property(woli => woli.TotalCost).HasPrecision(10, 2);
        builder.Property(woli => woli.LaborHours).HasPrecision(5, 2);

        // inventory item ID is optional
        builder.Property(woli => woli.InventoryItemID).IsRequired(false);
        builder.Property(woli => woli.AssignedToUserID)
               .IsRequired(false);

        // ✅ EXISTING INDEXES (keeping all as they are)
        builder.HasIndex(woli => woli.WorkOrderID);
        builder.HasIndex(woli => woli.InventoryItemID);
        builder.HasIndex(woli => woli.ServiceTaskID);
        builder.HasIndex(woli => woli.ItemType);
        builder.HasIndex(woli => woli.CreatedAt);
        builder.HasIndex(woli => woli.AssignedToUserID);

        // Composite indexes for common queries (existing)
        builder.HasIndex(woli => new { woli.WorkOrderID, woli.ItemType });

        // ✅ NEW PERFORMANCE INDEXES (adding only indexes)

        // Cost analysis indexes
        builder.HasIndex(woli => new { woli.WorkOrderID, woli.TotalCost })
            .HasDatabaseName("IX_WorkOrderLineItems_WorkOrderTotalCost");

        builder.HasIndex(woli => new { woli.ItemType, woli.TotalCost })
            .HasDatabaseName("IX_WorkOrderLineItems_ItemTypeTotalCost");

        // Labor tracking indexes
        builder.HasIndex(woli => new { woli.AssignedToUserID, woli.LaborHours })
            .HasDatabaseName("IX_WorkOrderLineItems_UserLaborHours");

        builder.HasIndex(woli => new { woli.ServiceTaskID, woli.LaborHours })
            .HasDatabaseName("IX_WorkOrderLineItems_ServiceTaskLaborHours");

        // Inventory usage indexes
        builder.HasIndex(woli => new { woli.InventoryItemID, woli.Quantity })
            .HasDatabaseName("IX_WorkOrderLineItems_InventoryItemQuantity");

        builder.HasIndex(woli => new { woli.InventoryItemID, woli.CreatedAt })
            .HasDatabaseName("IX_WorkOrderLineItems_InventoryItemCreatedAt");

        // Work order summary indexes
        builder.HasIndex(woli => new { woli.WorkOrderID, woli.ItemType, woli.TotalCost })
            .HasDatabaseName("IX_WorkOrderLineItems_WorkOrderItemTypeCost");

        builder.HasIndex(woli => new { woli.WorkOrderID, woli.CreatedAt })
            .HasDatabaseName("IX_WorkOrderLineItems_WorkOrderCreatedAt");

        // Covering indexes for common queries
        builder.HasIndex(woli => new { woli.WorkOrderID, woli.ItemType })
            .IncludeProperties(woli => new
            {
                woli.Quantity,
                woli.UnitPrice,
                woli.TotalCost,
                woli.LaborHours,
                woli.ServiceTaskID,
                woli.InventoryItemID
            })
            .HasDatabaseName("IX_WorkOrderLineItems_WorkOrderItemTypeCovering");

        builder.HasIndex(woli => new { woli.ServiceTaskID, woli.CreatedAt })
            .IncludeProperties(woli => new
            {
                woli.TotalCost,
                woli.LaborHours,
                woli.AssignedToUserID
            })
            .HasDatabaseName("IX_WorkOrderLineItems_ServiceTaskCreatedAtCovering");

        // User assignment tracking
        builder.HasIndex(woli => new { woli.AssignedToUserID, woli.CreatedAt })
            .HasDatabaseName("IX_WorkOrderLineItems_UserCreatedAt");

        // Financial analysis indexes
        builder.HasIndex(woli => new { woli.UnitPrice, woli.Quantity })
            .HasDatabaseName("IX_WorkOrderLineItems_UnitPriceQuantity");

        builder.HasIndex(woli => new { woli.HourlyRate, woli.LaborHours })
            .HasDatabaseName("IX_WorkOrderLineItems_HourlyRateLaborHours");

        // Check Constraints (keeping all existing)
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_Quantity",
            "Quantity > 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_UnitPrice",
            "UnitPrice IS NULL OR UnitPrice >= 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_HourlyRate",
            "HourlyRate IS NULL OR HourlyRate >= 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_LaborHours",
            "LaborHours IS NULL OR LaborHours > 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_TotalCost",
            "TotalCost >= 0"));

        // Table Relationships (keeping all existing exactly as they are)
        builder
            .HasOne(woli => woli.WorkOrder)
            .WithMany(wo => wo.WorkOrderLineItems)
            .HasForeignKey(woli => woli.WorkOrderID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(woli => woli.InventoryItem)
            .WithMany(i => i.WorkOrderLineItems)
            .HasForeignKey(woli => woli.InventoryItemID)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder
            .HasOne(woli => woli.ServiceTask)
            .WithMany(st => st.WorkOrderLineItems)
            .HasForeignKey(woli => woli.ServiceTaskID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(woli => woli.User)
            .WithMany()
            .HasForeignKey(woli => woli.AssignedToUserID)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
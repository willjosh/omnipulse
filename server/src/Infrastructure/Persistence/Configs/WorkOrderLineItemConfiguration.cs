using System;

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

        // Configure computed property
        builder.Property(woli => woli.TotalCost)
               .HasComputedColumnSql("Quantity * UnitCost");

        // Regular Indexes
        builder.HasIndex(woli => woli.WorkOrderID);
        builder.HasIndex(woli => woli.InventoryItemID);
        builder.HasIndex(woli => woli.ServiceTaskID);
        builder.HasIndex(woli => woli.ItemType);
        builder.HasIndex(woli => woli.CreatedAt);

        // Composite indexes for common queries
        builder.HasIndex(woli => new { woli.WorkOrderID, woli.ItemType });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_Quantity",
            "Quantity > 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_UnitPrice",
            "UnitPrice IS NULL OR UnitPrice >= 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_HourlyRate",
            "HourlyRate IS NULL OR HourlyRate >= 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_ServicePrice",
            "ServicePrice IS NULL OR ServicePrice >= 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_LaborHours",
            "LaborHours IS NULL OR LaborHours > 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_TotalCost",
            "TotalCost >= 0"));

        // Business rule constraints based on ItemType
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_PartsValidation",
            @"ItemType != 'PARTS' OR (InventoryItemID IS NOT NULL AND UnitPrice IS NOT NULL)"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_MaterialsValidation",
            @"ItemType != 'MATERIALS' OR (InventoryItemID IS NOT NULL AND UnitPrice IS NOT NULL)"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_LaborValidation",
            @"ItemType != 'LABOR' OR (HourlyRate IS NOT NULL AND LaborHours IS NOT NULL)"));

        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_ServiceValidation",
            @"ItemType != 'SERVICE' OR ServicePrice IS NOT NULL"));

        // Table Relationships
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
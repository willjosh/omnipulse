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
        builder.Property(woli => woli.UnitCost).HasPrecision(10, 2);

        // Configure computed property
        builder.Property(woli => woli.TotalCost)
               .HasComputedColumnSql("Quantity * UnitCost");

        // Regular Indexes
        builder.HasIndex(woli => woli.WorkOrderID);
        builder.HasIndex(woli => woli.InventoryItemID);
        builder.HasIndex(woli => woli.ServiceTaskID);
        builder.HasIndex(woli => woli.ItemType);

        // Composite indexes for common queries
        builder.HasIndex(woli => new { woli.WorkOrderID, woli.ItemType });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_Quantity",
            "Quantity > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_UnitCost",
            "UnitCost >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_LaborHours",
            "LaborHours IS NULL OR LaborHours >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_WorkOrderLineItem_SequenceNumber",
            "SequenceNumber > 0"));

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
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(woli => woli.ServiceTask)
            .WithMany(st => st.WorkOrderLineItems)
            .HasForeignKey(woli => woli.ServiceTaskID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        // Table & Key
        builder.ToTable("InventoryItems");
        builder.HasKey(i => i.ID);

        // Required properties
        builder.Property(i => i.ItemNumber)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(i => i.ItemName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(i => i.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Optional string properties with length constraints
        builder.Property(i => i.Description).HasMaxLength(500);
        builder.Property(i => i.Manufacturer).HasMaxLength(100);
        builder.Property(i => i.ManufacturerPartNumber).HasMaxLength(100);
        builder.Property(i => i.UniversalProductCode).HasMaxLength(12); // UPC-A: 12 digits
        builder.Property(i => i.Supplier).HasMaxLength(100);
        builder.Property(i => i.CompatibleVehicleTypes).HasMaxLength(200);

        builder.Property(i => i.UnitCost).HasPrecision(18, 2);

        builder.HasIndex(i => i.ItemNumber)
            .IsUnique()
            .HasDatabaseName("IX_InventoryItems_ItemNumber_Unique");

        builder.HasIndex(i => i.UniversalProductCode)
            .IsUnique()
            .HasDatabaseName("IX_InventoryItems_UniversalProductCode_Unique")
            .HasFilter("[UniversalProductCode] IS NOT NULL");

        // Composite uniqueness: Manufacturer + ManufacturerPartNumber (when both provided)
        builder.HasIndex(i => new { i.Manufacturer, i.ManufacturerPartNumber })
            .IsUnique()
            .HasDatabaseName("IX_InventoryItems_Manufacturer_PartNumber_Unique")
            .HasFilter("[Manufacturer] IS NOT NULL AND [ManufacturerPartNumber] IS NOT NULL");

        // Regular indexes with explicit names for performance
        builder.HasIndex(i => i.Category).HasDatabaseName("IX_InventoryItems_Category");
        builder.HasIndex(i => i.IsActive).HasDatabaseName("IX_InventoryItems_IsActive");
        builder.HasIndex(i => i.ItemName).HasDatabaseName("IX_InventoryItems_ItemName");
        builder.HasIndex(i => i.Manufacturer).HasDatabaseName("IX_InventoryItems_Manufacturer");

        // Composite index for common queries
        builder.HasIndex(i => new { i.IsActive, i.Category }).HasDatabaseName("IX_InventoryItems_IsActive_Category");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_InventoryItem_UnitCost_NonNegative",
                "([UnitCost] IS NULL OR [UnitCost] >= 0)");

            t.HasCheckConstraint("CK_InventoryItem_WeightKG_NonNegative",
                "([WeightKG] IS NULL OR [WeightKG] >= 0)");

            // More database-agnostic UPC validation (just length check)
            t.HasCheckConstraint("CK_InventoryItem_UPC_Length",
                "([UniversalProductCode] IS NULL OR LEN([UniversalProductCode]) = 12)");
        });

        // Navigation properties configuration (if needed)
        builder.HasMany(i => i.Inventories)
            .WithOne(inv => inv.InventoryItem)
            .HasForeignKey(inv => inv.InventoryItemID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.WorkOrderLineItems)
            .WithOne(woli => woli.InventoryItem)
            .HasForeignKey(woli => woli.InventoryItemID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

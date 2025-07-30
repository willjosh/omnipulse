using System;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        // Table configuration
        builder.ToTable("Inventories");
        builder.HasKey(i => i.ID);

        // Required properties with defaults
        builder.Property(i => i.InventoryItemID)
            .IsRequired();

        builder.Property(i => i.QuantityOnHand)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.MinStockLevel)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.MaxStockLevel)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(i => i.NeedsReorder)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.UnitCost)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        // Optional properties
        builder.Property(i => i.InventoryItemLocationID)
            .IsRequired(false);

        builder.Property(i => i.LastRestockedDate)
            .IsRequired(false);

        // Foreign Key Relationships
        builder.HasOne(i => i.InventoryItem)
            .WithMany(ii => ii.Inventories)
            .HasForeignKey(i => i.InventoryItemID)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

        builder.HasOne(i => i.InventoryItemLocation)
            .WithMany(l => l.Inventories)
            .HasForeignKey(i => i.InventoryItemLocationID)
            .OnDelete(DeleteBehavior.SetNull) // Allow location to be removed
            .IsRequired(false);

        builder.HasMany(i => i.InventoryTransactions)
            .WithOne(t => t.Inventory)
            .HasForeignKey(t => t.InventoryID)
            .OnDelete(DeleteBehavior.Cascade); // Delete transactions when inventory is deleted

        // INDEXES

        // Primary lookup indexes
        builder.HasIndex(i => i.InventoryItemID)
            .HasDatabaseName("IX_Inventories_InventoryItemID");

        builder.HasIndex(i => i.InventoryItemLocationID)
            .HasDatabaseName("IX_Inventories_InventoryItemLocationID");

        // Stock level monitoring indexes
        builder.HasIndex(i => i.NeedsReorder)
            .HasDatabaseName("IX_Inventories_NeedsReorder")
            .HasFilter("[NeedsReorder] = 1"); // This is valid - comparing to constant

        builder.HasIndex(i => new { i.QuantityOnHand, i.MinStockLevel })
            .HasDatabaseName("IX_Inventories_StockLevels");

        // Low stock composite index (FIXED - removed invalid filter)
        builder.HasIndex(i => new { i.QuantityOnHand, i.MinStockLevel, i.InventoryItemID })
            .HasDatabaseName("IX_Inventories_LowStock");
        // Removed: .HasFilter("[QuantityOnHand] <= [MinStockLevel]"); - Invalid column comparison

        // Location-based inventory queries
        builder.HasIndex(i => new { i.InventoryItemLocationID, i.InventoryItemID })
            .HasDatabaseName("IX_Inventories_Location_Item");

        builder.HasIndex(i => new { i.InventoryItemLocationID, i.QuantityOnHand })
            .HasDatabaseName("IX_Inventories_Location_Stock");

        // Restock tracking
        builder.HasIndex(i => i.LastRestockedDate)
            .HasDatabaseName("IX_Inventories_LastRestockedDate");

        builder.HasIndex(i => new { i.LastRestockedDate, i.InventoryItemID })
            .HasDatabaseName("IX_Inventories_RestockDate_Item");

        // Cost analysis indexes
        builder.HasIndex(i => i.UnitCost)
            .HasDatabaseName("IX_Inventories_UnitCost");

        builder.HasIndex(i => new { i.InventoryItemID, i.UnitCost })
            .HasDatabaseName("IX_Inventories_Item_Cost");

        // Audit/tracking indexes (from BaseEntity)
        builder.HasIndex(i => i.CreatedAt)
            .HasDatabaseName("IX_Inventories_CreatedAt");

        builder.HasIndex(i => i.UpdatedAt)
            .HasDatabaseName("IX_Inventories_UpdatedAt");

        // Composite indexes for common query patterns

        // Inventory summary by location
        builder.HasIndex(i => new { i.InventoryItemLocationID, i.QuantityOnHand, i.UnitCost })
            .HasDatabaseName("IX_Inventories_LocationSummary");

        // Item performance tracking
        builder.HasIndex(i => new { i.InventoryItemID, i.LastRestockedDate, i.QuantityOnHand })
            .HasDatabaseName("IX_Inventories_ItemPerformance");

        // Value analysis (total inventory value calculations)
        builder.HasIndex(i => new { i.QuantityOnHand, i.UnitCost, i.InventoryItemID })
            .HasDatabaseName("IX_Inventories_ValueAnalysis");

        // Stock movement tracking
        builder.HasIndex(i => new { i.UpdatedAt, i.QuantityOnHand, i.InventoryItemID })
            .HasDatabaseName("IX_Inventories_StockMovement");
    }
}
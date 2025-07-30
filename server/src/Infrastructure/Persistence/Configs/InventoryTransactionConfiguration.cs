using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        // Table & Key
        builder.ToTable("InventoryTransactions");
        builder.HasKey(t => t.ID);

        // Required properties
        builder.Property(t => t.InventoryID)
            .IsRequired();

        builder.Property(t => t.TransactionType)
            .IsRequired();

        builder.Property(t => t.Quantity)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.UnitCost)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(t => t.TotalCost)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(t => t.PerformedByUserID)
            .IsRequired();

        // Relationships
        builder.HasOne(t => t.Inventory)
            .WithMany(i => i.InventoryTransactions)
            .HasForeignKey(t => t.InventoryID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.PerformedByUserID)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(t => t.InventoryID)
            .HasDatabaseName("IX_InventoryTransactions_InventoryID");

        builder.HasIndex(t => t.TransactionType)
            .HasDatabaseName("IX_InventoryTransactions_TransactionType");

        builder.HasIndex(t => t.PerformedByUserID)
            .HasDatabaseName("IX_InventoryTransactions_PerformedByUserID");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_InventoryTransactions_CreatedAt");

        builder.HasIndex(t => t.UpdatedAt)
            .HasDatabaseName("IX_InventoryTransactions_UpdatedAt");

        // Check constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_InventoryTransaction_Quantity_Positive", "[Quantity] > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_InventoryTransaction_UnitCost_NonNegative", "[UnitCost] >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_InventoryTransaction_TotalCost_NonNegative", "[TotalCost] >= 0"));
    }
}
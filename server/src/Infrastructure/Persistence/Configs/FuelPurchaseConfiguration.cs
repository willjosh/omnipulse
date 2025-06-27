using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class FuelPurchaseConfiguration : IEntityTypeConfiguration<FuelPurchase>
{
    public void Configure(EntityTypeBuilder<FuelPurchase> builder)
    {
        builder.ToTable("FuelPurchases");
        builder.HasKey(fp => fp.ID);

        // String Length Constraints
        builder.Property(fp => fp.FuelStation).HasMaxLength(100);
        builder.Property(fp => fp.ReceiptNumber).HasMaxLength(50);
        builder.Property(fp => fp.Notes).HasMaxLength(500);

        // Precision for decimal/money fields
        builder.Property(fp => fp.PricePerUnit).HasPrecision(10, 4);
        builder.Property(fp => fp.TotalCost).HasPrecision(10, 2);

        // Regular Indexes
        builder.HasIndex(fp => fp.VehicleId);
        builder.HasIndex(fp => fp.PurchasedByUserId);
        builder.HasIndex(fp => fp.PurchaseDate);
        builder.HasIndex(fp => fp.ReceiptNumber).IsUnique();

        // Composite indexes for common queries
        builder.HasIndex(fp => new { fp.VehicleId, fp.PurchaseDate });
        builder.HasIndex(fp => new { fp.PurchaseDate, fp.VehicleId });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_FuelPurchase_OdometerReading", "OdometerReading >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_FuelPurchase_Volume", "Volume > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_FuelPurchase_PricePerUnit", "PricePerUnit > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_FuelPurchase_TotalCost", "TotalCost > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_FuelPurchase_PurchaseDate",
            "PurchaseDate >= '2000-01-01' AND PurchaseDate <= GETDATE()"));

        // Table Relationships
        builder
            .HasOne(fp => fp.Vehicle)
            .WithMany()
            .HasForeignKey(fp => fp.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(fp => fp.User)
            .WithMany()
            .HasForeignKey(fp => fp.PurchasedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
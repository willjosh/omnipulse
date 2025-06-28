using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class VehicleAlertConfiguration : IEntityTypeConfiguration<VehicleAlert>
{
    public void Configure(EntityTypeBuilder<VehicleAlert> builder)
    {
        builder.ToTable("VehicleAlerts");
        builder.HasKey(va => va.ID);

        // String Length Constraints
        builder.Property(va => va.Title).HasMaxLength(200);
        builder.Property(va => va.Message).HasMaxLength(1000);

        // Regular Indexes
        builder.HasIndex(va => va.VehicleID);
        builder.HasIndex(va => va.CreatedByUserID);
        builder.HasIndex(va => va.AcknowledgedByUserID);
        builder.HasIndex(va => va.AlertType);
        builder.HasIndex(va => va.AlertLevel);
        builder.HasIndex(va => va.IsAcknowledged);
        builder.HasIndex(va => va.IsDismissed);
        builder.HasIndex(va => va.ExpiresAt);
        builder.HasIndex(va => va.CreatedAt);

        // Composite indexes for common queries
        builder.HasIndex(va => new { va.VehicleID, va.IsAcknowledged });
        builder.HasIndex(va => new { va.CreatedByUserID, va.IsAcknowledged });
        builder.HasIndex(va => new { va.AlertLevel, va.IsAcknowledged });
        builder.HasIndex(va => new { va.IsAcknowledged, va.IsDismissed });
        builder.HasIndex(va => new { va.ExpiresAt, va.IsAcknowledged });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_VehicleAlert_AcknowledgedAt",
            "AcknowledgedAt IS NULL OR (IsAcknowledged = 1 AND AcknowledgedAt >= CreatedAt)"));
        builder.ToTable(t => t.HasCheckConstraint("CK_VehicleAlert_ExpiresAt",
            "ExpiresAt IS NULL OR ExpiresAt > CreatedAt"));
        builder.ToTable(t => t.HasCheckConstraint("CK_VehicleAlert_AcknowledgedByUserID",
            "AcknowledgedByUserID IS NULL OR IsAcknowledged = 1"));

        // Table Relationships
        builder
            .HasOne(va => va.Vehicle)
            .WithMany()
            .HasForeignKey(va => va.VehicleID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(va => va.User)
            .WithMany()
            .HasForeignKey(va => va.CreatedByUserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(va => va.User)
            .WithMany()
            .HasForeignKey(va => va.AcknowledgedByUserID)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
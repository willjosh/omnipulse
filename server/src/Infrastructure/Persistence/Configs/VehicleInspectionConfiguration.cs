using System;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class VehicleInspectionConfiguration : IEntityTypeConfiguration<VehicleInspection>
{
    public void Configure(EntityTypeBuilder<VehicleInspection> builder)
    {
        builder.ToTable("VehicleInspections");
        builder.HasKey(vi => vi.ID);

        // String Length Constraints
        builder.Property(vi => vi.Notes).HasMaxLength(2000);

        // Regular Indexes
        builder.HasIndex(vi => vi.VehicleID);
        builder.HasIndex(vi => vi.InspectionTypeID);
        builder.HasIndex(vi => vi.TechnicianID);
        builder.HasIndex(vi => vi.InspectionDate);
        builder.HasIndex(vi => vi.OverallStatus);
        builder.HasIndex(vi => vi.IsPassed);

        // Composite indexes for common queries
        builder.HasIndex(vi => new { vi.VehicleID, vi.InspectionDate });
        builder.HasIndex(vi => new { vi.InspectionTypeID, vi.InspectionDate });
        builder.HasIndex(vi => new { vi.TechnicianID, vi.InspectionDate });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_VehicleInspection_MileageAtInspection", "MileageAtInspection >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_VehicleInspection_Times", "EndTime >= StartTime"));
        builder.ToTable(t => t.HasCheckConstraint("CK_VehicleInspection_InspectionDate",
            "InspectionDate >= '2000-01-01' AND InspectionDate <= GETDATE()"));

        // Table Relationships
        builder
            .HasOne(vi => vi.Vehicle)
            .WithMany(v => v.VehicleInspections)
            .HasForeignKey(vi => vi.VehicleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(vi => vi.InspectionType)
            .WithMany(it => it.VehicleInspections)
            .HasForeignKey(vi => vi.InspectionTypeID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(vi => vi.User)
            .WithMany(u => u.VehicleInspections)
            .HasForeignKey(vi => vi.TechnicianID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
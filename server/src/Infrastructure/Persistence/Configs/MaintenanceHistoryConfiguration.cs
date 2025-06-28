using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class MaintenanceHistoryConfiguration : IEntityTypeConfiguration<MaintenanceHistory>
{
    public void Configure(EntityTypeBuilder<MaintenanceHistory> builder)
    {
        builder.ToTable("MaintenanceHistories");
        builder.HasKey(m => m.ID);

        // String length constraints
        builder.Property(m => m.Description).HasMaxLength(1000);
        builder.Property(m => m.Notes).HasMaxLength(1000);

        // Decimal precision constraints
        builder.Property(m => m.Cost).HasColumnType("decimal(18,2)");

        // Regular indexes
        builder.HasIndex(m => m.VehicleID);
        builder.HasIndex(m => m.WorkOrderID);
        builder.HasIndex(m => m.ServiceTaskID);
        builder.HasIndex(m => m.TechnicianID);
        builder.HasIndex(m => m.ServiceDate);
        builder.HasIndex(m => m.CreatedAt);

        // Check constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_MaintenanceHistory_Cost", "Cost >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_MaintenanceHistory_LabourHours", "LabourHours >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_MaintenanceHistory_MileageAtService", "MileageAtService >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_MaintenanceHistory_ServiceDate", "ServiceDate >= CreatedAt"));

        // Table relationships
        builder
            .HasOne(m => m.Vehicle)
            .WithMany()
            .HasForeignKey(m => m.VehicleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(m => m.WorkOrder)
            .WithMany()
            .HasForeignKey(m => m.WorkOrderID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

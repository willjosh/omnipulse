using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasKey(v => v.ID);
        
        // String length constraints
        builder.Property(v => v.Name).HasMaxLength(100);
        builder.Property(v => v.Make).HasMaxLength(30);
        builder.Property(v => v.Model).HasMaxLength(50);
        builder.Property(v => v.Trim).HasMaxLength(50);  
        builder.Property(v => v.Location).HasMaxLength(100);  
        builder.Property(v => v.VIN).HasMaxLength(17);
        builder.Property(v => v.LicensePlate).HasMaxLength(20);
        
        // Unique Constraints
        builder.HasIndex(v => v.VIN).IsUnique();
        builder.HasIndex(v => v.LicensePlate).IsUnique();
        
        // Regular indexes
        builder.HasIndex(v => v.AssignedTechnicianID);
        builder.HasIndex(v => v.VehicleGroupID);
        builder.HasIndex(v => v.Status);
        
        builder.ToTable(t => t.HasCheckConstraint("CK_Vehicle_Year", "Year > 1885 AND Year <= 2100"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Vehicle_Mileage", "Mileage >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Vehicle_EngineHours", "EngineHours >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Vehicle_FuelCapacity", "FuelCapacity > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Vehicle_PurchasePrice", "PurchasePrice >= 0"));
        
        // Table Relationships
        builder
            .HasOne(v => v.User)
            .WithMany(u => u.Vehicles)
            .HasForeignKey(v => v.AssignedTechnicianID)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder
            .HasOne(v => v.VehicleGroup)
            .WithMany(vg => vg.Vehicles)
            .HasForeignKey(v => v.VehicleGroupID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Domain.Entities;
using Domain.Entities.Enums;

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

        builder.HasIndex(v => v.VIN).IsUnique()
            .HasDatabaseName("UX_Vehicles_VIN");
        builder.HasIndex(v => v.LicensePlate).IsUnique()
            .HasDatabaseName("UX_Vehicles_LicensePlate");

        builder.HasIndex(v => v.AssignedTechnicianID)
            .HasDatabaseName("IX_Vehicles_AssignedTechnicianID");
        builder.HasIndex(v => v.VehicleGroupID)
            .HasDatabaseName("IX_Vehicles_VehicleGroupID");
        builder.HasIndex(v => v.Status)
            .HasDatabaseName("IX_Vehicles_Status");
        builder.HasIndex(v => v.VehicleType)
            .HasDatabaseName("IX_Vehicles_VehicleType");
        builder.HasIndex(v => v.Make)
            .HasDatabaseName("IX_Vehicles_Make");
        builder.HasIndex(v => v.Model)
            .HasDatabaseName("IX_Vehicles_Model");
        builder.HasIndex(v => v.Year)
            .HasDatabaseName("IX_Vehicles_Year");

        builder.HasIndex(v => v.AssignedTechnicianID)
            .HasFilter($"{nameof(Vehicle.AssignedTechnicianID)} IS NOT NULL")
            .HasDatabaseName("IX_Vehicles_AssignedTechnician_NotNull");

        builder.HasIndex(v => v.AssignedTechnicianID)
            .HasFilter($"{nameof(Vehicle.AssignedTechnicianID)} IS NULL")
            .HasDatabaseName("IX_Vehicles_AssignedTechnician_Null");

        // Status-specific filtered indexes for dashboard counts
        builder.HasIndex(v => v.Status)
            .HasFilter($"{nameof(Vehicle.Status)} = {(int)VehicleStatusEnum.ACTIVE}")
            .HasDatabaseName("IX_Vehicles_Status_Active");

        builder.HasIndex(v => v.Status)
            .HasFilter($"{nameof(Vehicle.Status)} = {(int)VehicleStatusEnum.MAINTENANCE}")
            .HasDatabaseName("IX_Vehicles_Status_Maintenance");

        builder.HasIndex(v => v.Status)
            .HasFilter($"{nameof(Vehicle.Status)} = {(int)VehicleStatusEnum.OUT_OF_SERVICE}")
            .HasDatabaseName("IX_Vehicles_Status_OutOfService");

        builder.HasIndex(v => v.Status)
            .HasFilter($"{nameof(Vehicle.Status)} = {(int)VehicleStatusEnum.INACTIVE}")
            .HasDatabaseName("IX_Vehicles_Status_Inactive");

        builder.HasIndex(v => new { v.Status, v.Mileage })
            .HasDatabaseName("IX_Vehicles_StatusMileage");

        builder.HasIndex(v => new { v.VehicleGroupID, v.Status })
            .HasDatabaseName("IX_Vehicles_GroupStatus");

        builder.HasIndex(v => new { v.VehicleGroupID, v.Status, v.Mileage })
            .HasDatabaseName("IX_Vehicles_GroupStatusMileage");

        builder.HasIndex(v => new { v.Make, v.Model, v.Year })
            .HasDatabaseName("IX_Vehicles_MakeModelYear");

        builder.HasIndex(v => new { v.VehicleType, v.Status })
            .HasDatabaseName("IX_Vehicles_TypeStatus");

        builder.HasIndex(v => new { v.AssignedTechnicianID, v.Status })
            .HasDatabaseName("IX_Vehicles_TechnicianStatus");

        builder.HasIndex(v => new { v.Status, v.CreatedAt })
            .IncludeProperties(v => new
            {
                v.Name,
                v.Make,
                v.Model,
                v.Year,
                v.VehicleType,
                v.VehicleGroupID,
                v.AssignedTechnicianID,
                v.Mileage
            })
            .HasDatabaseName("IX_Vehicles_StatusCreatedCovering");

        builder.HasIndex(v => new { v.VehicleGroupID, v.Name })
            .IncludeProperties(v => new
            {
                v.Status,
                v.Make,
                v.Model,
                v.Year,
                v.Mileage
            })
            .HasDatabaseName("IX_Vehicles_GroupNameCovering");

        builder.HasIndex(v => new { v.Name, v.Status })
            .HasDatabaseName("IX_Vehicles_NameStatus");

        builder.HasIndex(v => new { v.VIN, v.Status })
            .HasDatabaseName("IX_Vehicles_VINStatus");

        builder.HasIndex(v => new { v.LicensePlate, v.Status })
            .HasDatabaseName("IX_Vehicles_LicensePlateStatus");

        builder.HasIndex(v => new { v.PurchaseDate, v.Status })
            .HasDatabaseName("IX_Vehicles_PurchaseDateStatus");

        builder.HasIndex(v => new { v.LicensePlateExpirationDate, v.Status })
            .HasDatabaseName("IX_Vehicles_LicenseExpirationStatus");

        builder.HasIndex(v => new { v.CreatedAt, v.Status })
            .HasDatabaseName("IX_Vehicles_CreatedAtStatus");

        builder.HasIndex(v => new { v.Mileage, v.Status })
            .HasDatabaseName("IX_Vehicles_MileageStatus");

        builder.HasIndex(v => new { v.EngineHours, v.Status })
            .HasDatabaseName("IX_Vehicles_EngineHoursStatus");

        builder.HasIndex(v => new { v.PurchasePrice, v.Status })
            .HasDatabaseName("IX_Vehicles_PurchasePriceStatus");

        // Check Constraints
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
            .WithMany()
            .HasForeignKey(v => v.VehicleGroupID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
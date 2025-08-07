using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class XrefServiceProgramVehicleConfiguration : IEntityTypeConfiguration<XrefServiceProgramVehicle>
{
    public void Configure(EntityTypeBuilder<XrefServiceProgramVehicle> builder)
    {
        builder.ToTable("XrefServiceProgramVehicles");

        // Composite Primary Key
        builder.HasKey(xspv => new { xspv.ServiceProgramID, xspv.VehicleID });

        // Non-unique Indexes
        builder.HasIndex(xspv => xspv.ServiceProgramID);
        builder.HasIndex(xspv => xspv.VehicleID);
        builder.HasIndex(xspv => xspv.AddedAt);

        // Add indexes for service reminder generation
        builder.HasIndex(xspv => new { xspv.ServiceProgramID, xspv.AddedAt })
            .HasDatabaseName("IX_XrefServiceProgramVehicles_ProgramAdded");

        builder.HasIndex(xspv => new { xspv.VehicleID, xspv.AddedAt })
            .HasDatabaseName("IX_XrefServiceProgramVehicles_VehicleAdded");

        // Table Relationships
        // XrefServiceProgramVehicles N:1 ServiceProgram
        builder
            .HasOne(xspv => xspv.ServiceProgram)
            .WithMany(sp => sp.XrefServiceProgramVehicles)
            .HasForeignKey(xspv => xspv.ServiceProgramID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(xspv => xspv.Vehicle)
            .WithMany(v => v.XrefServiceProgramVehicles)
            .HasForeignKey(xspv => xspv.VehicleID)
            .OnDelete(DeleteBehavior.Cascade);

        // builder
        //     .HasOne(xspv => xspv.User)
        //     .WithMany()
        //     .HasForeignKey(xspv => xspv.AddedByUserID)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}
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

        // Table Relationships
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
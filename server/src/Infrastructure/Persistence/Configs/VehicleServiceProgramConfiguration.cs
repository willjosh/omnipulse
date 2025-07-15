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
        builder.HasKey(vsp => new { vsp.VehicleID, vsp.ServiceProgramID });

        // Regular Indexes
        builder.HasIndex(vsp => vsp.AssignedDate);
        builder.HasIndex(vsp => vsp.IsActive);

        // Composite indexes for common queries
        builder.HasIndex(vsp => new { vsp.VehicleID, vsp.IsActive });
        builder.HasIndex(vsp => new { vsp.ServiceProgramID, vsp.IsActive });
        builder.HasIndex(vsp => new { vsp.IsActive, vsp.AssignedDate });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_XrefServiceProgramVehicle_AssignedDate",
            "AssignedDate >= '2000-01-01' AND AssignedDate <= GETDATE()"));

        // Table Relationships
        builder
            .HasOne(vsp => vsp.Vehicle)
            .WithMany(v => v.XrefServiceProgramVehicles)
            .HasForeignKey(vsp => vsp.VehicleID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(vsp => vsp.ServiceProgram)
            .WithMany()
            .HasForeignKey(vsp => vsp.ServiceProgramID)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
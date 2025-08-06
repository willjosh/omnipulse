using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class VehicleGroupConfiguration : IEntityTypeConfiguration<VehicleGroup>
{
    public void Configure(EntityTypeBuilder<VehicleGroup> builder)
    {
        builder.ToTable("VehicleGroups");
        builder.HasKey(vg => vg.ID);

        // String Length Constraints
        builder.Property(vg => vg.Name).HasMaxLength(100);
        builder.Property(vg => vg.Description).HasMaxLength(300);

        builder.HasIndex(vg => vg.IsActive)
            .HasDatabaseName("IX_VehicleGroups_IsActive");

        builder.HasIndex(vg => vg.IsActive)
            .IncludeProperties(vg => new { vg.Name, vg.Description })
            .HasDatabaseName("IX_VehicleGroups_ActiveCovering");

        builder.HasIndex(vg => new { vg.Name, vg.IsActive })
            .HasDatabaseName("IX_VehicleGroups_NameActive");

        builder.HasIndex(vg => new { vg.CreatedAt, vg.IsActive })
            .HasDatabaseName("IX_VehicleGroups_CreatedAtActive");
    }
}
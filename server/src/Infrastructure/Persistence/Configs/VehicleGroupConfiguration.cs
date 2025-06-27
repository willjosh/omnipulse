using System;
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

        // Unique Constraint
        builder.HasIndex(vg => vg.Name).IsUnique();

        // Regular Indexes
        builder.HasIndex(vg => vg.IsActive);

        // String Length Constraints
        builder.Property(vg => vg.Name).HasMaxLength(100);
        builder.Property(vg => vg.Description).HasMaxLength(300);

    }
}

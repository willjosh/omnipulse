using System;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class ServiceProgramConfiguration : IEntityTypeConfiguration<ServiceProgram>
{
    public void Configure(EntityTypeBuilder<ServiceProgram> builder)
    {
        builder.ToTable("ServicePrograms");
        builder.HasKey(sp => sp.ID);

        // String Length Constraints
        builder.Property(sp => sp.Name).HasMaxLength(200);

        // Unique Constraints
        builder.HasIndex(sp => sp.Name).IsUnique();

        // Regular Indexes
        builder.HasIndex(sp => sp.IsActive);

        // Query Filter for soft deletes
        builder.HasQueryFilter(sp => sp.IsActive);

        // Relationships & Delete Behavior
        builder
            .HasMany(sp => sp.ServiceSchedules)
            .WithOne(ss => ss.ServiceProgram)
            .HasForeignKey(ss => ss.ServiceProgramID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(sp => sp.XrefServiceProgramVehicles)
            .WithOne(xspv => xspv.ServiceProgram)
            .HasForeignKey(xspv => xspv.ServiceProgramID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
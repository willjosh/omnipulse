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
        builder.Property(sp => sp.OEMTag).HasMaxLength(100);

        // Unique Constraints
        builder.HasIndex(sp => sp.Name).IsUnique();
        builder.HasIndex(sp => sp.OEMTag).IsUnique();

        // Regular Indexes
        builder.HasIndex(sp => sp.IsActive);

        // Composite indexes for common queries
        builder.HasIndex(sp => new { sp.OEMTag, sp.IsActive });

        // Query Filter for soft deletes
        builder.HasQueryFilter(sp => sp.IsActive);
    }
}
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

        // Index for active service programs with name
        builder.HasIndex(sp => sp.IsActive)
            .IncludeProperties(sp => sp.Name)
            .HasDatabaseName("IX_ServicePrograms_ActiveWithName");
    }
}
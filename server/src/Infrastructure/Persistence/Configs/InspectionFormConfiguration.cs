using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InspectionFormConfiguration : IEntityTypeConfiguration<InspectionForm>
{
    public void Configure(EntityTypeBuilder<InspectionForm> builder)
    {
        builder.ToTable("InspectionForms");
        builder.HasKey(f => f.ID);

        // String Length Constraints
        builder.Property(f => f.Title).HasMaxLength(200);
        builder.Property(f => f.Description).HasMaxLength(1000);

        // Regular Indexes
        builder.HasIndex(f => f.CreatedAt);
        builder.HasIndex(f => f.IsActive);

        // Unique Constraint
        builder.HasIndex(f => f.Title).IsUnique();
    }
}
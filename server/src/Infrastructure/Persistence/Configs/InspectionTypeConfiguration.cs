using Domain.Entities;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InspectionTypeConfiguration : IEntityTypeConfiguration<InspectionType>
{
    public void Configure(EntityTypeBuilder<InspectionType> builder)
    {
        builder.ToTable("InspectionTypes");
        builder.HasKey(it => it.ID);

        // String Length Constraints
        builder.Property(it => it.Name).HasMaxLength(100);
        builder.Property(it => it.Description).HasMaxLength(1000);

        // Regular Indexes
        builder.HasIndex(it => it.CreatedAt);

        // Unique Constraint
        builder.HasIndex(it => it.Name).IsUnique();
    }
}

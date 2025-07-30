using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InspectionFormItemConfiguration : IEntityTypeConfiguration<InspectionFormItem>
{
    public void Configure(EntityTypeBuilder<InspectionFormItem> builder)
    {
        builder.ToTable("InspectionFormItems");
        builder.HasKey(fi => fi.ID);

        // String Length Constraints
        builder.Property(fi => fi.ItemLabel).HasMaxLength(200);
        builder.Property(fi => fi.ItemDescription).HasMaxLength(500);

        // Regular Indexes
        builder.HasIndex(fi => fi.InspectionFormID);
        builder.HasIndex(fi => fi.IsRequired);

        // Unique constraint to prevent duplicate items per inspection form
        builder.HasIndex(fi => new { fi.InspectionFormID, fi.ItemLabel }).IsUnique();

        // Table Relationships
        builder
            .HasOne(fi => fi.InspectionForm)
            .WithMany(f => f.InspectionFormItems)
            .HasForeignKey(fi => fi.InspectionFormID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
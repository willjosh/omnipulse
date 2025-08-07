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
        builder.Property(fi => fi.ItemInstructions).HasMaxLength(4000);

        // ✅ EXISTING INDEXES (keeping all as they are)
        builder.HasIndex(fi => fi.InspectionFormID);
        builder.HasIndex(fi => fi.IsRequired);

        // Unique constraint to prevent duplicate items per inspection form
        builder.HasIndex(fi => new { fi.InspectionFormID, fi.ItemLabel }).IsUnique();

        // ✅ NEW PERFORMANCE INDEXES (adding only indexes)

        // Form item retrieval optimization
        builder.HasIndex(fi => new { fi.InspectionFormID, fi.IsActive })
            .HasDatabaseName("IX_InspectionFormItems_FormActive");

        builder.HasIndex(fi => new { fi.InspectionFormID, fi.IsRequired })
            .HasDatabaseName("IX_InspectionFormItems_FormRequired");

        builder.HasIndex(fi => new { fi.InspectionFormID, fi.InspectionFormItemTypeEnum })
            .HasDatabaseName("IX_InspectionFormItems_FormType");

        // Covering indexes for form item queries
        builder.HasIndex(fi => new { fi.InspectionFormID, fi.IsActive })
            .IncludeProperties(fi => new
            {
                fi.ItemLabel,
                fi.ItemDescription,
                fi.IsRequired,
                fi.InspectionFormItemTypeEnum,
                fi.CreatedAt
            })
            .HasDatabaseName("IX_InspectionFormItems_FormActiveCovering");

        // Search and filtering indexes
        builder.HasIndex(fi => new { fi.ItemLabel, fi.InspectionFormID })
            .HasDatabaseName("IX_InspectionFormItems_LabelForm");

        builder.HasIndex(fi => new { fi.IsRequired, fi.IsActive })
            .HasDatabaseName("IX_InspectionFormItems_RequiredActive");

        builder.HasIndex(fi => fi.InspectionFormItemTypeEnum)
            .HasDatabaseName("IX_InspectionFormItems_ItemType");

        // Active items optimization
        builder.HasIndex(fi => fi.IsActive)
            .HasFilter($"{nameof(InspectionFormItem.IsActive)} = 1")
            .HasDatabaseName("IX_InspectionFormItems_Active");

        // Required items optimization
        builder.HasIndex(fi => new { fi.InspectionFormID, fi.IsRequired })
            .HasFilter($"{nameof(InspectionFormItem.IsRequired)} = 1")
            .HasDatabaseName("IX_InspectionFormItems_FormRequired_True");

        // Audit and creation tracking
        builder.HasIndex(fi => new { fi.CreatedAt, fi.InspectionFormID })
            .HasDatabaseName("IX_InspectionFormItems_CreatedAtForm");

        builder.HasIndex(fi => new { fi.UpdatedAt, fi.IsActive })
            .HasDatabaseName("IX_InspectionFormItems_UpdatedAtActive");

        // Table Relationships (keeping as is)
        builder
            .HasOne(fi => fi.InspectionForm)
            .WithMany(f => f.InspectionFormItems)
            .HasForeignKey(fi => fi.InspectionFormID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
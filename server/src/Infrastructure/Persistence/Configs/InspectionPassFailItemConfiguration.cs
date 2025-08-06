using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InspectionPassFailItemConfiguration : IEntityTypeConfiguration<InspectionPassFailItem>
{
    public void Configure(EntityTypeBuilder<InspectionPassFailItem> builder)
    {
        builder.ToTable("InspectionPassFailItems");

        // Composite Primary Key
        builder.HasKey(pfi => new { pfi.InspectionID, pfi.InspectionFormItemID });

        // String Length Constraints
        builder.Property(pfi => pfi.Comment).HasMaxLength(1000);
        builder.Property(pfi => pfi.SnapshotItemLabel).HasMaxLength(200);
        builder.Property(pfi => pfi.SnapshotItemDescription).HasMaxLength(500);
        builder.Property(pfi => pfi.SnapshotItemInstructions).HasMaxLength(4000);

        // ✅ EXISTING INDEXES (keeping all as they are)
        builder.HasIndex(pfi => pfi.InspectionID);
        builder.HasIndex(pfi => pfi.InspectionFormItemID);
        builder.HasIndex(pfi => pfi.Passed);

        // ✅ NEW PERFORMANCE INDEXES (adding only indexes)

        // Pass/Fail analysis indexes
        builder.HasIndex(pfi => new { pfi.InspectionID, pfi.Passed })
            .HasDatabaseName("IX_InspectionPassFailItems_InspectionPassed");

        builder.HasIndex(pfi => new { pfi.InspectionFormItemID, pfi.Passed })
            .HasDatabaseName("IX_InspectionPassFailItems_FormItemPassed");

        builder.HasIndex(pfi => new { pfi.Passed, pfi.SnapshotIsRequired })
            .HasDatabaseName("IX_InspectionPassFailItems_PassedRequired");

        // Covering indexes for inspection results
        builder.HasIndex(pfi => pfi.InspectionID)
            .IncludeProperties(pfi => new
            {
                pfi.InspectionFormItemID,
                pfi.Passed,
                pfi.Comment,
                pfi.SnapshotItemLabel,
                pfi.SnapshotIsRequired
            })
            .HasDatabaseName("IX_InspectionPassFailItems_InspectionCovering");

        builder.HasIndex(pfi => new { pfi.InspectionID, pfi.Passed })
            .IncludeProperties(pfi => new
            {
                pfi.SnapshotItemLabel,
                pfi.Comment,
                pfi.SnapshotIsRequired
            })
            .HasDatabaseName("IX_InspectionPassFailItems_InspectionPassedCovering");

        // Failed items tracking indexes
        builder.HasIndex(pfi => pfi.Passed)
            .HasFilter($"{nameof(InspectionPassFailItem.Passed)} = 0")
            .HasDatabaseName("IX_InspectionPassFailItems_Failed");

        builder.HasIndex(pfi => new { pfi.InspectionID, pfi.Passed })
            .HasFilter($"{nameof(InspectionPassFailItem.Passed)} = 0")
            .HasDatabaseName("IX_InspectionPassFailItems_InspectionFailed");

        // Required items tracking
        builder.HasIndex(pfi => new { pfi.SnapshotIsRequired, pfi.Passed })
            .HasDatabaseName("IX_InspectionPassFailItems_RequiredPassed");

        builder.HasIndex(pfi => new { pfi.InspectionID, pfi.SnapshotIsRequired })
            .HasFilter($"{nameof(InspectionPassFailItem.SnapshotIsRequired)} = 1")
            .HasDatabaseName("IX_InspectionPassFailItems_InspectionRequired");

        // Item type tracking
        builder.HasIndex(pfi => pfi.SnapshotInspectionFormItemTypeEnum)
            .HasDatabaseName("IX_InspectionPassFailItems_ItemType");

        builder.HasIndex(pfi => new { pfi.SnapshotInspectionFormItemTypeEnum, pfi.Passed })
            .HasDatabaseName("IX_InspectionPassFailItems_TypePassed");

        // Search and filtering indexes
        builder.HasIndex(pfi => new { pfi.SnapshotItemLabel, pfi.Passed })
            .HasDatabaseName("IX_InspectionPassFailItems_LabelPassed");

        builder.HasIndex(pfi => new { pfi.Comment, pfi.Passed })
            .HasDatabaseName("IX_InspectionPassFailItems_CommentPassed");

        // Analysis indexes for reports
        builder.HasIndex(pfi => new { pfi.InspectionFormItemID, pfi.Passed, pfi.SnapshotIsRequired })
            .HasDatabaseName("IX_InspectionPassFailItems_FormItemPassedRequired");

        // Performance monitoring indexes
        builder.HasIndex(pfi => new { pfi.InspectionFormItemID, pfi.SnapshotIsRequired })
            .IncludeProperties(pfi => new { pfi.Passed, pfi.SnapshotItemLabel })
            .HasDatabaseName("IX_InspectionPassFailItems_FormItemRequiredCovering");

        // Table Relationships (keeping as is)
        builder
            .HasOne(pfi => pfi.Inspection)
            .WithMany(i => i.InspectionPassFailItems)
            .HasForeignKey(pfi => pfi.InspectionID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(pfi => pfi.InspectionFormItem)
            .WithMany()
            .HasForeignKey(pfi => pfi.InspectionFormItemID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
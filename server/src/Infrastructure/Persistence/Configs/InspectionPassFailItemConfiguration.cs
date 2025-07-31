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

        // Regular Indexes
        builder.HasIndex(pfi => pfi.Passed);

        // Table Relationships
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
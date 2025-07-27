using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InspectionAttachmentConfiguration : IEntityTypeConfiguration<InspectionAttachment>
{
    public void Configure(EntityTypeBuilder<InspectionAttachment> builder)
    {
        builder.ToTable("InspectionAttachments");
        builder.HasKey(ia => ia.ID);

        // String Length Constraints
        builder.Property(ia => ia.FileName).HasMaxLength(255);
        builder.Property(ia => ia.FilePath).HasMaxLength(500);
        builder.Property(ia => ia.Description).HasMaxLength(500);

        // Regular Indexes
        builder.HasIndex(ia => ia.VehicleInspectionID);
        builder.HasIndex(ia => ia.CheckListItemID);
        builder.HasIndex(ia => ia.FileType);
        builder.HasIndex(ia => ia.CreatedAt);

        // Composite index for common queries
        builder.HasIndex(ia => new { ia.VehicleInspectionID, ia.CheckListItemID });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_InspectionAttachment_FileSize", "FileSize > 0"));

        // Table Relationships
        builder
            .HasOne(ia => ia.VehicleInspection)
            .WithMany()
            .HasForeignKey(ia => ia.VehicleInspectionID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ia => ia.CheckListItem)
            .WithMany()
            .HasForeignKey(ia => ia.CheckListItemID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
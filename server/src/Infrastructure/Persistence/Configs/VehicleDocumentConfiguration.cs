using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class VehicleDocumentConfiguration : IEntityTypeConfiguration<VehicleDocument>
{
    public void Configure(EntityTypeBuilder<VehicleDocument> builder)
    {
        builder.ToTable("VehicleDocuments");
        builder.HasKey(vd => vd.ID);

        // String Length Constraints
        builder.Property(vd => vd.Title).HasMaxLength(100);
        builder.Property(vd => vd.FileName).HasMaxLength(500);
        builder.Property(vd => vd.FilePath).HasMaxLength(500);
        builder.Property(vd => vd.Notes).HasMaxLength(1000);
        builder.HasIndex(vd => vd.ExpiryDate);
        builder.HasIndex(vd => vd.CreatedAt);

        // Regular Indexes
        builder.HasIndex(vd => vd.DocumentType);

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_VehicleDocument_FileSize", "FileSize > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_VehicleDocument_ExpiryDate",
            "ExpiryDate IS NULL OR ExpiryDate >= CreatedAt"));

        // Table Relationships
        builder
            .HasOne(vd => vd.Vehicle)
            .WithMany(vd => vd.VehicleDocuments)
            .HasForeignKey(vd => vd.VehicleID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(vd => vd.User)
            .WithMany()
            .HasForeignKey(vd => vd.UploadedByUserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

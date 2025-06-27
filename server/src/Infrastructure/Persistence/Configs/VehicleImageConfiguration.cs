using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class VehicleImageConfiguration : IEntityTypeConfiguration<VehicleImage>
{
    public void Configure(EntityTypeBuilder<VehicleImage> builder)
    {
        builder.ToTable("VehicleImages");
        builder.HasKey(vi => vi.ID);

        // String Length Constraints
        builder.Property(vi => vi.ImageLabel).HasMaxLength(100);
        builder.Property(vi => vi.FileName).HasMaxLength(255);
        builder.Property(vi => vi.FilePath).HasMaxLength(500);

        // Regular Indexes
        builder.HasIndex(vi => vi.VehicleID);
        builder.HasIndex(vi => vi.UploadedBy);
        builder.HasIndex(vi => vi.CreatedAt);

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_VehicleImage_FileSize", "FileSize > 0"));

        // Table Relationships
        builder
            .HasOne(vi => vi.Vehicle)
            .WithMany(v => v.VehicleImages)
            .HasForeignKey(vi => vi.VehicleID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(vi => vi.User)
            .WithMany()
            .HasForeignKey(vi => vi.UploadedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
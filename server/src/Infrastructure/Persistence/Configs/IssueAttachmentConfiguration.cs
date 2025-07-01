using System;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class IssueAttachmentConfiguration : IEntityTypeConfiguration<IssueAttachment>
{
    public void Configure(EntityTypeBuilder<IssueAttachment> builder)
    {
        builder.ToTable("IssueAttachments");
        builder.HasKey(ia => ia.ID);

        // String Length Constraints
        builder.Property(ia => ia.FileName).HasMaxLength(255);
        builder.Property(ia => ia.FilePath).HasMaxLength(500);
        builder.Property(ia => ia.Description).HasMaxLength(500);

        // Regular Indexes
        builder.HasIndex(ia => ia.IssueID);
        builder.HasIndex(ia => ia.UploadedByUserID);
        builder.HasIndex(ia => ia.FileType);
        builder.HasIndex(ia => ia.CreatedAt);

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_IssueAttachment_FileSize", "FileSize > 0"));

        // Table Relationships
        builder
            .HasOne(ia => ia.Issue)
            .WithMany(i => i.IssueAttachments)
            .HasForeignKey(ia => ia.IssueID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ia => ia.User)
            .WithMany()
            .HasForeignKey(ia => ia.UploadedByUserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
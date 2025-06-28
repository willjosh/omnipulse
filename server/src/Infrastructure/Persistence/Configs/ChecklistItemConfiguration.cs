using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class ChecklistItemConfiguration : IEntityTypeConfiguration<CheckListItem>
{
    public void Configure(EntityTypeBuilder<CheckListItem> builder)
    {
        builder.ToTable("CheckListItems");
        builder.HasKey(cli => cli.ID);

        // String Length Constraints
        builder.Property(cli => cli.Category).HasMaxLength(100);
        builder.Property(cli => cli.ItemName).HasMaxLength(200);
        builder.Property(cli => cli.Description).HasMaxLength(500);

        // Regular Indexes
        builder.HasIndex(cli => cli.InspectionTypeID);
        builder.HasIndex(cli => cli.Category);
        builder.HasIndex(cli => cli.IsMandatory);

        // Unique constraint to prevent duplicate items per inspection type
        builder.HasIndex(cli => new { cli.InspectionTypeID, cli.ItemName }).IsUnique();

        // Table Relationships
        builder
            .HasOne(cli => cli.Inspection)
            .WithMany()
            .HasForeignKey(cli => cli.InspectionTypeID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
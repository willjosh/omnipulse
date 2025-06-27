using System;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.ToTable("Issues");
        builder.HasKey(i => i.ID);

        // String Length Constraints
        builder.Property(i => i.Title).HasMaxLength(200);
        builder.Property(i => i.Description).HasMaxLength(1000);
        builder.Property(i => i.ResolutionNotes).HasMaxLength(1000);

        // Unique Constraints
        builder.HasIndex(i => i.IssueNumber).IsUnique();

        // Regular Indexes (for performance)
        builder.HasIndex(i => i.VehicleID);
        builder.HasIndex(i => i.ReportedByUserID);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.PriorityLevel);
        builder.HasIndex(i => i.Category);
        builder.HasIndex(i => i.CreatedAt);

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_Issue_ResolvedDate",
            "ResolvedDate IS NULL OR ResolvedDate >= CreatedAt"));

        builder
            .HasOne(i => i.Vehicle)
            .WithMany()
            .HasForeignKey(i => i.VehicleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.ReportedByUserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
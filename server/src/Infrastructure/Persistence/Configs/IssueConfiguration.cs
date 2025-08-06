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

        // 1. Basic single-column indexes (keep existing ones)
        builder.HasIndex(i => i.VehicleID)
            .HasDatabaseName("IX_Issues_VehicleID");

        builder.HasIndex(i => i.ReportedByUserID)
            .HasDatabaseName("IX_Issues_ReportedByUserID");

        builder.HasIndex(i => i.Status)
            .HasDatabaseName("IX_Issues_Status");

        builder.HasIndex(i => i.PriorityLevel)
            .HasDatabaseName("IX_Issues_PriorityLevel");

        builder.HasIndex(i => i.Category)
            .HasDatabaseName("IX_Issues_Category");


        builder.HasIndex(i => new { i.Status, i.PriorityLevel, i.ReportedDate })
            .HasDatabaseName("IX_Issues_StatusPriorityReported");

        builder.HasIndex(i => new { i.VehicleID, i.Status, i.ReportedDate })
            .HasDatabaseName("IX_Issues_VehicleStatusReported");

        builder.HasIndex(i => new { i.ReportedByUserID, i.Status, i.ReportedDate })
            .HasDatabaseName("IX_Issues_UserStatusReported");

        builder.HasIndex(i => new { i.Status, i.ReportedDate })
            .IncludeProperties(i => new
            {
                i.Title,
                i.PriorityLevel,
                i.Category,
                i.VehicleID,
                i.ReportedByUserID
            })
            .HasDatabaseName("IX_Issues_StatusReportedCovering");

        builder.HasIndex(i => new { i.Title, i.Status })
            .HasDatabaseName("IX_Issues_TitleStatus");

        builder.HasIndex(i => new { i.ReportedDate, i.Status })
            .IncludeProperties(i => new { i.PriorityLevel, i.Category })
            .HasDatabaseName("IX_Issues_ReportedDateStatusCovering");

        builder.HasIndex(i => new { i.ResolvedDate, i.Status })
            .HasDatabaseName("IX_Issues_ResolvedDateStatus");

        builder.HasIndex(i => new { i.ResolvedByUserID, i.ResolvedDate })
            .HasDatabaseName("IX_Issues_ResolvedByDateResolved");

        builder.HasIndex(i => new { i.CreatedAt, i.Status })
            .HasDatabaseName("IX_Issues_CreatedAtStatus");

        builder.HasIndex(i => new { i.UpdatedAt, i.Status })
            .HasDatabaseName("IX_Issues_UpdatedAtStatus");

        builder.HasIndex(i => new { i.InspectionID, i.Status })
            .HasFilter($"{nameof(Issue.InspectionID)} IS NOT NULL")
            .HasDatabaseName("IX_Issues_InspectionStatus");

        builder.HasIndex(i => i.Title)
            .IncludeProperties(i => new { i.Description, i.Status, i.PriorityLevel })
            .HasDatabaseName("IX_Issues_TitleWithDetails");

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_Issue_ResolvedDate",
            $"{nameof(Issue.ResolvedDate)} IS NULL OR {nameof(Issue.ResolvedDate)} >= {nameof(Issue.ReportedDate)}"));

        // Foreign Key Relationships
        builder
            .HasOne(i => i.Vehicle)
            .WithMany(v => v.Issues)
            .HasForeignKey(i => i.VehicleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(i => i.ReportedByUser)
            .WithMany()
            .HasForeignKey(i => i.ReportedByUserID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(i => i.ResolvedByUser)
            .WithMany()
            .HasForeignKey(i => i.ResolvedByUserID)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
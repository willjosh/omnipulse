using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class WorkOrderIssueConfiguration : IEntityTypeConfiguration<WorkOrderIssue>
{
    public void Configure(EntityTypeBuilder<WorkOrderIssue> builder)
    {
        builder.ToTable("WorkOrderIssues");
        builder.HasKey(woi => new { woi.WorkOrderID, woi.IssueID });

        // ✅ BASIC INDEXES
        builder.HasIndex(woi => woi.WorkOrderID)
            .HasDatabaseName("IX_WorkOrderIssues_WorkOrderID");

        builder.HasIndex(woi => woi.IssueID)
            .HasDatabaseName("IX_WorkOrderIssues_IssueID");

        builder.HasIndex(woi => woi.AssignedDate)
            .HasDatabaseName("IX_WorkOrderIssues_AssignedDate");

        // ✅ COMPOSITE INDEXES
        builder.HasIndex(woi => new { woi.WorkOrderID, woi.AssignedDate })
            .HasDatabaseName("IX_WorkOrderIssues_WorkOrderAssignedDate");

        builder.HasIndex(woi => new { woi.IssueID, woi.AssignedDate })
            .HasDatabaseName("IX_WorkOrderIssues_IssueAssignedDate");

        // ✅ COVERING INDEXES
        builder.HasIndex(woi => woi.WorkOrderID)
            .IncludeProperties(woi => new { woi.IssueID, woi.AssignedDate })
            .HasDatabaseName("IX_WorkOrderIssues_WorkOrderCovering");

        builder.HasIndex(woi => woi.IssueID)
            .IncludeProperties(woi => new { woi.WorkOrderID, woi.AssignedDate })
            .HasDatabaseName("IX_WorkOrderIssues_IssueCovering");

        // Foreign Key Relationships
        builder
            .HasOne(woi => woi.WorkOrder)
            .WithMany()
            .HasForeignKey(woi => woi.WorkOrderID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(woi => woi.Issue)
            .WithMany()
            .HasForeignKey(woi => woi.IssueID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
using System;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class WorkOrderIssueConfiguration : IEntityTypeConfiguration<WorkOrderIssue>
{
    public void Configure(EntityTypeBuilder<WorkOrderIssue> builder)
    {
        builder.ToTable("WorkOrderIssues");

        builder.HasKey(wi => new { wi.WorkOrderID, wi.IssueID });

        // Regular Indexes 
        builder.HasIndex(wi => wi.WorkOrderID);
        builder.HasIndex(wi => wi.IssueID);

        // Table Relationships
        builder
            .HasOne(wi => wi.WorkOrder)
            .WithMany()
            .HasForeignKey(wi => wi.WorkOrderID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(wi => wi.Issue)
            .WithMany()
            .HasForeignKey(wi => wi.IssueID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
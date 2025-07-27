using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class IssueAssignmentConfiguration : IEntityTypeConfiguration<IssueAssignment>
{
    public void Configure(EntityTypeBuilder<IssueAssignment> builder)
    {
        builder.ToTable("IssueAssignments");
        builder.HasKey(ia => new { ia.IssueID, ia.AssignedToUserID });

        // Property Constraints
        builder.Property(ia => ia.Notes).HasMaxLength(1000);
        builder.Property(ia => ia.IsActive).IsRequired();
        builder.Property(ia => ia.AssignedDate).IsRequired();

        // Indexes
        builder.HasIndex(ia => ia.IssueID);
        builder.HasIndex(ia => ia.AssignedToUserID);
        builder.HasIndex(ia => ia.IsActive);
        builder.HasIndex(ia => ia.AssignedDate);

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_IssueAssignment_UnassignedDate",
            "UnassignedDate IS NULL OR UnassignedDate >= AssignedDate"));

        // Relationships
        builder
            .HasOne(ia => ia.Issue)
            .WithMany(i => i.IssueAssignments)
            .HasForeignKey(ia => ia.IssueID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ia => ia.User)
            .WithMany()
            .HasForeignKey(ia => ia.AssignedToUserID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using System;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InspectionChecklistResponseConfiguration : IEntityTypeConfiguration<InspectionChecklistResponse>
{
    public void Configure(EntityTypeBuilder<InspectionChecklistResponse> builder)
    {
        builder.ToTable("InspectionChecklistResponses");

        builder.HasKey(icr => new { icr.VehicleInspectionID, icr.ChecklistItemID });

        // String Length Constraints
        builder.Property(icr => icr.TextResponse).HasMaxLength(500);
        builder.Property(icr => icr.Note).HasMaxLength(1000);

        // Regular Indexes
        builder.HasIndex(icr => icr.Status);
        builder.HasIndex(icr => icr.RequiresAttention);
        builder.HasIndex(icr => icr.ResponseDate);

        // Table Relationships
        builder
            .HasOne(icr => icr.VehicleInspection)
            .WithMany()
            .HasForeignKey(icr => icr.VehicleInspectionID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(icr => icr.CheckListItem)
            .WithMany()
            .HasForeignKey(icr => icr.ChecklistItemID)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
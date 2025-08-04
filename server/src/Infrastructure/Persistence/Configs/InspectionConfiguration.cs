using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
{
    public void Configure(EntityTypeBuilder<Inspection> builder)
    {
        builder.ToTable("Inspections");
        builder.HasKey(i => i.ID);

        // String Length Constraints
        builder.Property(i => i.Notes).HasMaxLength(2000);

        // Regular Indexes
        builder.HasIndex(i => i.VehicleID);
        builder.HasIndex(i => i.InspectionFormID);
        builder.HasIndex(i => i.TechnicianID);
        builder.HasIndex(i => i.InspectionStartTime);
        builder.HasIndex(i => i.VehicleCondition);

        // Composite indexes for common queries
        builder.HasIndex(i => new { i.VehicleID, i.InspectionStartTime });
        builder.HasIndex(i => new { i.InspectionFormID, i.InspectionStartTime });
        builder.HasIndex(i => new { i.TechnicianID, i.InspectionStartTime });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_Inspection_OdometerReading", "OdometerReading >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Inspection_Times", "InspectionEndTime >= InspectionStartTime"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Inspection_InspectionStartTime",
            "InspectionStartTime >= '2000-01-01' AND InspectionStartTime <= GETDATE()"));

        // Table Relationships
        builder
            .HasOne(i => i.Vehicle)
            .WithMany(v => v.Inspections)
            .HasForeignKey(i => i.VehicleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(i => i.InspectionForm)
            .WithMany(f => f.Inspections)
            .HasForeignKey(i => i.InspectionFormID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(i => i.User)
            .WithMany(u => u.Inspections)
            .HasForeignKey(i => i.TechnicianID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
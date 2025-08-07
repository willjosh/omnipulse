using Domain.Entities;
using Domain.Entities.Enums;

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
        builder.Property(i => i.SnapshotFormTitle).HasMaxLength(200);
        builder.Property(i => i.SnapshotFormDescription).HasMaxLength(1000);

        // ✅ EXISTING INDEXES (keeping all as they are)
        builder.HasIndex(i => i.VehicleID)
            .HasDatabaseName("IX_Inspections_VehicleID");
        builder.HasIndex(i => i.InspectionFormID)
            .HasDatabaseName("IX_Inspections_InspectionFormID");
        builder.HasIndex(i => i.TechnicianID)
            .HasDatabaseName("IX_Inspections_TechnicianID");
        builder.HasIndex(i => i.InspectionStartTime)
            .HasDatabaseName("IX_Inspections_InspectionStartTime");
        builder.HasIndex(i => i.VehicleCondition)
            .HasDatabaseName("IX_Inspections_VehicleCondition");

        // ✅ EXISTING COMPOSITE INDEXES (keeping all as they are)
        builder.HasIndex(i => new { i.VehicleID, i.InspectionStartTime })
            .HasDatabaseName("IX_Inspections_VehicleStartTime");
        builder.HasIndex(i => new { i.InspectionFormID, i.InspectionStartTime })
            .HasDatabaseName("IX_Inspections_FormStartTime");
        builder.HasIndex(i => new { i.TechnicianID, i.InspectionStartTime })
            .HasDatabaseName("IX_Inspections_TechnicianStartTime");

        // ✅ NEW PERFORMANCE INDEXES (adding only indexes)

        // Date-based indexes for reporting and scheduling
        builder.HasIndex(i => i.InspectionEndTime)
            .HasDatabaseName("IX_Inspections_InspectionEndTime");

        builder.HasIndex(i => new { i.InspectionStartTime, i.InspectionEndTime })
            .HasDatabaseName("IX_Inspections_StartEndTime");

        builder.HasIndex(i => new { i.InspectionStartTime, i.VehicleCondition })
            .HasDatabaseName("IX_Inspections_StartTimeCondition");

        // Vehicle condition tracking indexes
        builder.HasIndex(i => new { i.VehicleCondition, i.InspectionStartTime })
            .HasDatabaseName("IX_Inspections_ConditionStartTime");

        builder.HasIndex(i => new { i.VehicleID, i.VehicleCondition })
            .HasDatabaseName("IX_Inspections_VehicleCondition");

        // Technician performance indexes
        builder.HasIndex(i => new { i.TechnicianID, i.VehicleCondition })
            .HasDatabaseName("IX_Inspections_TechnicianCondition");

        builder.HasIndex(i => new { i.TechnicianID, i.InspectionEndTime })
            .HasDatabaseName("IX_Inspections_TechnicianEndTime");

        // Odometer tracking indexes
        builder.HasIndex(i => i.OdometerReading)
            .HasDatabaseName("IX_Inspections_OdometerReading");

        builder.HasIndex(i => new { i.VehicleID, i.OdometerReading })
            .HasDatabaseName("IX_Inspections_VehicleOdometer");

        builder.HasIndex(i => new { i.OdometerReading, i.InspectionStartTime })
            .HasDatabaseName("IX_Inspections_OdometerStartTime");

        // Form usage tracking indexes
        builder.HasIndex(i => new { i.InspectionFormID, i.VehicleCondition })
            .HasDatabaseName("IX_Inspections_FormCondition");

        builder.HasIndex(i => new { i.InspectionFormID, i.TechnicianID })
            .HasDatabaseName("IX_Inspections_FormTechnician");

        // Covering indexes for list views
        builder.HasIndex(i => new { i.InspectionStartTime, i.CreatedAt })
            .IncludeProperties(i => new
            {
                i.VehicleID,
                i.TechnicianID,
                i.VehicleCondition,
                i.InspectionFormID,
                i.OdometerReading,
                i.SnapshotFormTitle
            })
            .HasDatabaseName("IX_Inspections_StartTimeCreatedCovering");

        builder.HasIndex(i => new { i.VehicleID, i.InspectionStartTime })
            .IncludeProperties(i => new
            {
                i.TechnicianID,
                i.VehicleCondition,
                i.InspectionEndTime,
                i.OdometerReading,
                i.SnapshotFormTitle
            })
            .HasDatabaseName("IX_Inspections_VehicleStartTimeCovering");

        builder.HasIndex(i => new { i.TechnicianID, i.InspectionStartTime })
            .IncludeProperties(i => new
            {
                i.VehicleID,
                i.VehicleCondition,
                i.InspectionEndTime,
                i.OdometerReading,
                i.SnapshotFormTitle
            })
            .HasDatabaseName("IX_Inspections_TechnicianStartTimeCovering");

        // Search performance indexes
        builder.HasIndex(i => new { i.SnapshotFormTitle, i.InspectionStartTime })
            .HasDatabaseName("IX_Inspections_FormTitleStartTime");

        builder.HasIndex(i => new { i.Notes, i.InspectionStartTime })
            .HasDatabaseName("IX_Inspections_NotesStartTime");

        // Audit and filtering indexes
        builder.HasIndex(i => new { i.CreatedAt, i.VehicleCondition })
            .HasDatabaseName("IX_Inspections_CreatedAtCondition");

        builder.HasIndex(i => new { i.UpdatedAt, i.InspectionStartTime })
            .HasDatabaseName("IX_Inspections_UpdatedAtStartTime");

        // Vehicle condition filtered indexes for dashboard
        builder.HasIndex(i => i.VehicleCondition)
            .HasFilter($"{nameof(Inspection.VehicleCondition)} = {(int)VehicleConditionEnum.Excellent}")
            .HasDatabaseName("IX_Inspections_Condition_Excellent");

        builder.HasIndex(i => i.VehicleCondition)
            .HasFilter($"{nameof(Inspection.VehicleCondition)} = {(int)VehicleConditionEnum.NotSafeToOperate}")
            .HasDatabaseName("IX_Inspections_Condition_NotSafe");

        // Date range indexes for reports
        builder.HasIndex(i => new { i.InspectionStartTime, i.InspectionEndTime, i.VehicleCondition })
            .HasDatabaseName("IX_Inspections_DateRangeCondition");

        // Check Constraints (keeping all existing)
        builder.ToTable(t => t.HasCheckConstraint("CK_Inspection_OdometerReading", "OdometerReading >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Inspection_Times", "InspectionEndTime >= InspectionStartTime"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Inspection_InspectionStartTime",
            "InspectionStartTime >= '2000-01-01' AND InspectionStartTime <= GETDATE()"));

        // Table Relationships (keeping all existing exactly as they are)
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
            .WithMany()
            .HasForeignKey(i => i.TechnicianID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
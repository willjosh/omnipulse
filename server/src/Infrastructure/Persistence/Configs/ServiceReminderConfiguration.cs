using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class ServiceReminderConfiguration : IEntityTypeConfiguration<ServiceReminder>
{
    public void Configure(EntityTypeBuilder<ServiceReminder> builder)
    {
        builder.ToTable("ServiceReminders");
        builder.HasKey(sr => sr.ID);

        // Regular Indexes
        builder.HasIndex(sr => sr.VehicleID);
        builder.HasIndex(sr => sr.ServiceScheduleID);
        builder.HasIndex(sr => sr.DueDate);
        builder.HasIndex(sr => sr.Status);
        builder.HasIndex(sr => sr.PriorityLevel);

        // Composite indexes for common queries
        builder.HasIndex(sr => new { sr.DueDate, sr.VehicleID });
        builder.HasIndex(sr => new { sr.DueDate, sr.Status });
        builder.HasIndex(sr => new { sr.DueMileage, sr.VehicleID });
        builder.HasIndex(sr => new { sr.DueMileage, sr.Status });
        builder.HasIndex(sr => new { sr.DueDate, sr.DueMileage, sr.VehicleID, sr.Status });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_DueMileage", "DueMileage >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_CompletedDate",
            "CompletedDate IS NULL OR CompletedDate >= CreatedAt"));

        // XOR Constraint: Must be either time-based OR mileage-based, not both or neither
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_ScheduleType_XOR",
            "(" +
            "(TimeIntervalValue IS NOT NULL AND TimeIntervalUnit IS NOT NULL AND MileageInterval IS NULL) OR " +
            "(MileageInterval IS NOT NULL AND TimeIntervalValue IS NULL AND TimeIntervalUnit IS NULL)" +
            ")"
        ));

        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_DueDate_Required_If_TimeBased",
            "(TimeIntervalValue IS NULL OR DueDate IS NOT NULL)"));

        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_DueMileage_Required_If_MileageBased",
            "(MileageInterval IS NULL OR DueMileage IS NOT NULL)"));

        // Table Relationships
        builder
            .HasOne(sr => sr.Vehicle)
            .WithMany(v => v.ServiceReminders)
            .HasForeignKey(sr => sr.VehicleID)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(sr => sr.ServiceSchedule)
            .WithMany()
            .HasForeignKey(sr => sr.ServiceScheduleID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
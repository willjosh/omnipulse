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

        // String Length Constraints
        builder.Property(sr => sr.CancelReason).HasMaxLength(500);

        // Regular Indexes
        builder.HasIndex(sr => sr.VehicleID);
        builder.HasIndex(sr => sr.ServiceScheduleID);
        builder.HasIndex(sr => sr.WorkOrderID);
        builder.HasIndex(sr => sr.DueDate);
        builder.HasIndex(sr => sr.DueMileage);
        builder.HasIndex(sr => sr.Status);

        // Composite indexes for common queries (as suggested)
        builder.HasIndex(sr => new { sr.VehicleID, sr.Status })
            .HasDatabaseName("IX_ServiceReminders_VehicleID_Status");

        builder.HasIndex(sr => new { sr.ServiceScheduleID, sr.Status })
            .HasDatabaseName("IX_ServiceReminders_ServiceScheduleID_Status");

        // Additional performance indexes
        builder.HasIndex(sr => new { sr.DueDate, sr.Status });
        builder.HasIndex(sr => new { sr.DueMileage, sr.Status });
        builder.HasIndex(sr => new { sr.Status, sr.DueDate });
        builder.HasIndex(sr => new { sr.Status, sr.DueMileage });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_DueMileage",
            "DueMileage IS NULL OR DueMileage >= 0"));

        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_CompletedDate",
            "CompletedDate IS NULL OR CompletedDate >= CreatedAt"));

        // At least one due target must be set
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_HasDueTarget",
            "DueDate IS NOT NULL OR DueMileage IS NOT NULL"));

        // Unique constraint: Only one UPCOMING reminder per vehicle-schedule pair
        builder.HasIndex(sr => new { sr.VehicleID, sr.ServiceScheduleID, sr.Status })
            .IsUnique()
            .HasFilter("[Status] = 'UPCOMING'")
            .HasDatabaseName("IX_ServiceReminders_VehicleID_ServiceScheduleID_Status_Unique");

        // Concurrency control
        builder.Property(sr => sr.UpdatedAt)
            .IsConcurrencyToken();

        // Enum conversions
        builder.Property(sr => sr.Status).HasConversion<string>();

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
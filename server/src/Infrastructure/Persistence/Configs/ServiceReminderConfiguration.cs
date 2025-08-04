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
        builder.HasIndex(sr => new { sr.VehicleID, sr.DueDate });
        builder.HasIndex(sr => new { sr.Status, sr.DueDate });

        // Check Constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_DueMileage", "DueMileage >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_CompletedDate",
            "CompletedDate IS NULL OR CompletedDate >= CreatedAt"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceReminder_DueDate", "DueDate >= CreatedAt"));

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
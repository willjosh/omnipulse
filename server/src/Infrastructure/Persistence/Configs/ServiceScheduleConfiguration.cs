using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class ServiceScheduleConfiguration : IEntityTypeConfiguration<ServiceSchedule>
{
    public void Configure(EntityTypeBuilder<ServiceSchedule> builder)
    {
        builder.ToTable("ServiceSchedules");
        builder.HasKey(ss => ss.ID);

        // String Length Constraints
        builder.Property(ss => ss.Name).HasMaxLength(200);

        // Enum Configurations
        builder.Property(ss => ss.TimeIntervalUnit).HasConversion<string>();
        builder.Property(ss => ss.TimeBufferUnit).HasConversion<string>();

        // Regular Indexes
        builder.HasIndex(ss => ss.ServiceProgramID);
        builder.HasIndex(ss => ss.IsActive);
        builder.HasIndex(ss => ss.Name);
        builder.HasIndex(ss => ss.IsSoftDeleted);

        // Global Query Filter: exclude soft-deleted schedules
        builder.HasQueryFilter(ss => !ss.IsSoftDeleted);

        // Composite indexes for common queries
        builder.HasIndex(ss => new { ss.ServiceProgramID, ss.IsActive });
        builder.HasIndex(ss => new { ss.IsActive, ss.Name });

        // Unique constraint within service program
        builder.HasIndex(ss => new { ss.ServiceProgramID, ss.Name }).IsUnique();

        // Check Constraints for positive values
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_TimeIntervalValue",
            "TimeIntervalValue > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_TimeBufferValue",
            "TimeBufferValue >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_MileageInterval",
            "MileageInterval > 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_MileageBuffer",
            "MileageBuffer >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_FirstServiceMileage",
            "FirstServiceMileage >= 0"));

        // XOR Constraint: Exactly one of time-based OR mileage-based must be configured
        builder.ToTable(t => t.HasCheckConstraint("CK_ServiceSchedule_XOR_Constraint",
            "(TimeIntervalValue IS NULL AND TimeIntervalUnit IS NULL AND MileageInterval IS NOT NULL)"));

        // Table Relationships
        // ServiceSchedule N:1 ServiceProgram
        builder
            .HasOne(ss => ss.ServiceProgram)
            .WithMany(sp => sp.ServiceSchedules)
            .HasForeignKey(ss => ss.ServiceProgramID)
            .OnDelete(DeleteBehavior.Cascade);

        // Add these performance indexes for service reminder queries
        builder.HasIndex(ss => new { ss.IsActive, ss.TimeIntervalValue, ss.TimeIntervalUnit })
            .HasDatabaseName("IX_ServiceSchedules_TimeBasedActive");

        builder.HasIndex(ss => new { ss.IsActive, ss.MileageInterval })
            .HasDatabaseName("IX_ServiceSchedules_MileageBasedActive");

        builder.HasIndex(ss => new { ss.ServiceProgramID, ss.IsActive, ss.TimeIntervalValue })
            .HasDatabaseName("IX_ServiceSchedules_ProgramTimeActive");

        builder.HasIndex(ss => new { ss.ServiceProgramID, ss.IsActive, ss.MileageInterval })
            .HasDatabaseName("IX_ServiceSchedules_ProgramMileageActive");
    }
}
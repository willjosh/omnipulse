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
            "(TimeIntervalValue IS NOT NULL AND TimeIntervalUnit IS NOT NULL AND MileageInterval IS NULL) OR " +
            "(TimeIntervalValue IS NULL AND TimeIntervalUnit IS NULL AND MileageInterval IS NOT NULL)"));

        // Table Relationships
        // ServiceSchedule N:1 ServiceProgram
        builder
            .HasOne(ss => ss.ServiceProgram)
            .WithMany(sp => sp.ServiceSchedules)
            .HasForeignKey(ss => ss.ServiceProgramID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
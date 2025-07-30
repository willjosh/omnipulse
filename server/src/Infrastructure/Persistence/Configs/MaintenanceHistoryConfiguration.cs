using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configs;

public class MaintenanceHistoryConfiguration : IEntityTypeConfiguration<MaintenanceHistory>
{
    public void Configure(EntityTypeBuilder<MaintenanceHistory> builder)
    {
        builder.ToTable("MaintenanceHistories");
        builder.HasKey(m => m.ID);

        // Regular indexes
        builder.HasIndex(m => m.WorkOrderID);
        builder.HasIndex(m => m.ServiceDate);
        builder.HasIndex(m => m.CreatedAt);

        // Check constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_MaintenanceHistory_ServiceDate", "ServiceDate >= CreatedAt"));

        // Table relationships
        builder
            .HasOne(m => m.WorkOrder)
            .WithMany()
            .HasForeignKey(m => m.WorkOrderID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
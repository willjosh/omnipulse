using System;

using Domain.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.DatabaseContext;

public class OmnipulseDatabaseContext(DbContextOptions<OmnipulseDatabaseContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<CheckListItem> ChecklistItems { get; set; }
    public DbSet<FuelPurchase> FuelPurchases { get; set; }
    public DbSet<InspectionAttachment> InspectionAttachments { get; set; }
    public DbSet<InspectionChecklistResponse> InspectionChecklistResponses { get; set; }
    public DbSet<InspectionType> InspectionTypes { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<InventoryItemLocation> InventoryItemLocations { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<IssueAssignment> IssueAssignments { get; set; }
    public DbSet<IssueAttachment> IssueAttachments { get; set; }
    public DbSet<MaintenanceHistory> MaintenanceHistories { get; set; }
    public DbSet<ServiceProgram> ServicePrograms { get; set; }
    public DbSet<ServiceReminder> ServiceReminders { get; set; }
    public DbSet<ServiceSchedule> ServiceSchedules { get; set; }
    public DbSet<ServiceScheduleTask> ServiceScheduleTasks { get; set; }
    public DbSet<ServiceTask> ServiceTasks { get; set; }
    public DbSet<VehicleAlert> VehicleAlerts { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleDocument> VehicleDocuments { get; set; }
    public DbSet<VehicleGroup> VehicleGroups { get; set; }
    public DbSet<VehicleImage> VehicleImages { get; set; }
    public DbSet<VehicleInspection> VehicleInspections { get; set; }
    public DbSet<VehicleServiceProgram> VehicleServicePrograms { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<WorkOrderIssue> WorkOrderIssues { get; set; }
    public DbSet<WorkOrderLineItem> WorkOrderLineItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(OmnipulseDatabaseContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = base.ChangeTracker.Entries<BaseEntity>()
            .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            entry.Entity.UpdatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
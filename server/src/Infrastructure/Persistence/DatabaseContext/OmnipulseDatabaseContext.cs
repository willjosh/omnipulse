using System;

using Domain.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.DatabaseContext;

/// <summary>
/// Entity Framework Core <see cref="DbContext"/> for the application.
/// Inherits from <see cref="IdentityDbContext{TUser}"/> to integrate ASP.NET Core Identity support for <see cref="User"/>.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>Registers <see cref="DbSet{TEntity}"/> properties for all aggregate roots in the domain.</item>
/// <item>Applies entity configurations discovered via <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/> in <see cref="OnModelCreating"/>.</item>
/// <item>Overrides <see cref="SaveChangesAsync(CancellationToken)"/> to automatically maintain <see cref="BaseEntity.CreatedAt"/> and <see cref="BaseEntity.UpdatedAt"/> timestamps.</item>
/// </list>
/// </remarks>
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
    public DbSet<IssueAttachment> IssueAttachments { get; set; }
    public DbSet<Issue> Issues { get; set; }
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

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(OmnipulseDatabaseContext).Assembly);
    }

    /// <summary>
    /// Persists changes asynchronously whilst automatically updating auditing timestamps.
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
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
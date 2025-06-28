using System;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime HireDate { get; set; }
    public required Boolean IsActive { get; set; } = true;
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }

    // navigation properties
    public required ICollection<WorkOrder> WorkOrders { get; set; } = [];
    public required ICollection<MaintenanceHistory> MaintenanceHistories { get; set; } = [];
    public required ICollection<IssueAttachment> IssueAttachments { get; set; } = [];
    public required ICollection<VehicleAlert> VehicleAlerts { get; set; } = [];
    public required ICollection<VehicleAssignment> VehicleAssignments { get; set; } = [];
    public required ICollection<VehicleInspection> VehicleInspections { get; set; } = [];
    public required ICollection<Vehicle> Vehicles { get; set; } = [];
}
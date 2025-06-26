using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class ServiceReminder : BaseEntity
{
    // FKs
    public required int VehicleID { get; set; }
    public required int ServiceScheduleID { get; set; }

    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime DueDate { get; set; }
    public required double DueMileage { get; set; }
    public required double DueEngineHours { get; set; }
    public required PriorityLevelEnum PriorityLevel { get; set; }
    public required ReminderStatusEnum Status { get; set; }
    public required bool IsCompleted { get; set; } = false;
    public DateTime? CompletedDate { get; set; }
    public DateTime? LastNotificationSentDate { get; set; }
    public required int NotificationCount { get; set; } = 0;

    // Navigation properties
    public required ICollection<WorkOrder> WorkOrders { get; set; } = [];
    public required ServiceSchedule ServiceSchedule { get; set; }
    public required Vehicle Vehicle { get; set; }
}

using System;

namespace Domain.Entities;
using Domain.Entities.Enums;

public class VehicleAlert : BaseEntity
{
    // FKs
    public required int VehicleID { get; set; }
    public required int UserID { get; set; }

    public required AlertTypeEnum AlertType { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public required AlertLevelEnum AlertLevel { get; set; }

    public required bool IsAcknowledged { get; set; } = false;
    public int? AcknowledgedByUserID { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public required bool IsDismissed { get; set; } = false;

    // Navigation Properties
    public required Vehicle Vehicle { get; set; }
    public required User User { get; set; }
}

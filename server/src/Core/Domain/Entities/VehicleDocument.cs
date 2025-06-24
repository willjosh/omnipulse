using System;

namespace Domain.Entities;
using Domain.Entities.Enums;

public class VehicleDocument : BaseEntity
{
    // FKs
    public required int VehicleID { get; set; }
    public required int UploadedByUserID { get; set; }

    public required DocumentTypeEnum DocumentType { get; set; }
    public required string Title { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required int FileSize { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }

    // Navigation Properties
    public required Vehicle Vehicle { get; set; }
    // public required User User { get; set; }
}

namespace Domain.Entities;

public class VehicleImage : BaseEntity
{
    // FKs
    public required int VehicleID { get; set; }
    public required string UploadedByUserID { get; set; }

    // Image
    public required string ImageLabel { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required int FileSize { get; set; } // bytes

    // Navigation Properties
    public required Vehicle Vehicle { get; set; }
    public required User User { get; set; }
}
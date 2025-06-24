using System;

namespace Domain.Entities;
using Domain.Entities.Enums;

public class IssueAttachment : BaseEntity
{
    public required int IssueID { get; set; }
    public required int UploadedBy { get; set; }
    public required AttachmentTypeEnum FileType { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required int FileSize { get; set; }
    public string? Description { get; set; }

    // Navigation Properties
    public required Issue Issue { get; set; }

    // TODO: Connects to User
}

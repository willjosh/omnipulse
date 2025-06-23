using System;

namespace Domain.Entities;
using Domain.Entities.Enums;

public class IssueAttachment
{
    public int IssueId { get; set; }
    public int UploadedBy { get; set; }

    public AttachmentTypeEnum FileType { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public int FileSize { get; set; }
    public string? Description { get; set; }

    // Navigation Properties
    
}

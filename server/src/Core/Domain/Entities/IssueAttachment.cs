using System;

namespace Domain.Entities;
using Domain.Entities.Enums;

public class IssueAttachment
{
    public int IssueId { get; set; }
    public int UploadedBy { get; set; }

    public AttachmentTypeEnum FileType { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int FileSize { get; set; }
    public string? Description { get; set; }

    // Navigation Properties
    
}

namespace ContosoDashboard.Services;

public class DocumentDto
{
    public int DocumentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public int UploadedByUserId { get; set; }
    public string UploadedByName { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public bool IsPreviewable { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public string ScanStatus { get; set; } = string.Empty;
    public bool IsSharedWithMe { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
}

public class DocumentUploadRequest
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ProjectId { get; set; }
    public List<string>? Tags { get; set; }
    public Stream FileStream { get; set; } = Stream.Null;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public int UploadedByUserId { get; set; }
}

public class DocumentMetadataUpdate
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ProjectId { get; set; }
    public List<string>? Tags { get; set; }
}

public class DocumentShareRequest
{
    public int RecipientUserId { get; set; }
    public string? RecipientTeam { get; set; }
    public int SharedByUserId { get; set; }
    public string AccessType { get; set; } = "View";
}

public class DocumentQueryParameters
{
    public string? SearchText { get; set; }
    public string? Category { get; set; }
    public int? ProjectId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public int PageSize { get; set; } = 50;
    public int PageNumber { get; set; } = 1;
}

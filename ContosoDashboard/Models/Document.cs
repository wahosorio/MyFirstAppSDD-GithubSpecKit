using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public enum DocumentScanStatus
{
    Pending,
    Scanning,
    Safe,
    Blocked
}

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public int? ProjectId { get; set; }

    [Required]
    public int UploaderId { get; set; }

    [Required]
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string FileType { get; set; } = string.Empty;

    [Required]
    public long FileSize { get; set; }

    [MaxLength(1000)]
    public string? Tags { get; set; }

    [Required]
    public DocumentScanStatus ScanStatus { get; set; } = DocumentScanStatus.Pending;

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    [ForeignKey("UploaderId")]
    public virtual User Uploader { get; set; } = null!;

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    public virtual ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
    public virtual ICollection<DocumentActivity> Activities { get; set; } = new List<DocumentActivity>();
    public virtual ICollection<DocumentTaskAttachment> TaskAttachments { get; set; } = new List<DocumentTaskAttachment>();

    [NotMapped]
    public bool IsPreviewable => FileType.StartsWith("image/") || FileType == "application/pdf";
}

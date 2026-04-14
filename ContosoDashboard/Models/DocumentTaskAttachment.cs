using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class DocumentTaskAttachment
{
    [Key]
    public int DocumentTaskAttachmentId { get; set; }

    [Required]
    public int DocumentId { get; set; }

    [Required]
    public int TaskId { get; set; }

    [Required]
    public int AttachedByUserId { get; set; }

    [Required]
    public DateTime AttachedDate { get; set; } = DateTime.UtcNow;

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; } = null!;

    [ForeignKey("TaskId")]
    public virtual TaskItem Task { get; set; } = null!;

    [ForeignKey("AttachedByUserId")]
    public virtual User AttachedByUser { get; set; } = null!;
}

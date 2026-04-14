using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class DocumentShare
{
    [Key]
    public int DocumentShareId { get; set; }

    [Required]
    public int DocumentId { get; set; }

    public int? RecipientUserId { get; set; }

    [MaxLength(255)]
    public string? RecipientTeam { get; set; }

    [Required]
    public int SharedByUserId { get; set; }

    [Required]
    public DateTime SharedDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(50)]
    public string AccessType { get; set; } = "View";

    [ForeignKey("DocumentId")]
    public virtual Document Document { get; set; } = null!;

    [ForeignKey("RecipientUserId")]
    public virtual User? RecipientUser { get; set; }

    [ForeignKey("SharedByUserId")]
    public virtual User SharedByUser { get; set; } = null!;
}

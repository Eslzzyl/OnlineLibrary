using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.Model;

public class Recommend
{
    [Required]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    [Required]
    [MaxLength(100)]
    public string UserId { get; set; } = default!;

    [MaxLength(100)]
    public string? AdminId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = default!;

    [MaxLength(100)]
    public string? Author { get; set; }

    [MaxLength(100)]
    public string? Publisher { get; set; }

    [MaxLength(20)]
    public string? Isbn { get; set; }

    [MaxLength(200)]
    public string? UserRemark { get; set; }

    [Required]
    public bool IsProcessed { get; set; }

    [MaxLength(200)]
    public string? AdminRemark { get; set; }

    [Required]
    public DateTime CreateTime { get; set; }

    public DateTime UpdateTime { get; set; }
}
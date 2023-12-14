using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.Model;

public class BookComment
{
    [Required]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [Required]
    [MaxLength(100)]
    public string UserId { get; set; } = default!;
    
    [Required]
    public int BookId { get; set; }
    
    public int RefCommentId { get; set; }
    
    [Required]
    [MaxLength(400)]
    public string Content { get; set; } = default!;

    [Required]
    public DateTime CreateTime { get; set; }
    
    public ApiUser User { get; set; } = default!;
}
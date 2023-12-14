using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.Model;

public class BorrowHistory
{
    [Required]
    [ForeignKey("User")]
    public required string UserId { get; set; }

    [Required]
    [ForeignKey("Book")]
    public int BookId { get; set; }

    [Required]
    public DateTime BorrowDate { get; set; }

    [Required]
    public DateTime ReturnDate { get; set; }

    [Required]
    public Book Book { get; set; } = default!;

    [Required]
    public ApiUser User { get; set; } = default!;
}
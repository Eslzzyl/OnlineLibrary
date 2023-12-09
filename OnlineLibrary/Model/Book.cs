using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.Model;

public class Book
{
    [Required]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = default!;
    
    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = default!;
    
    [Required]
    [MaxLength(100)]
    public string Publisher { get; set; } = default!;
    
    [Required]
    [MaxLength(50)]
    public string PublishedDate { get; set; } = default!;
    
    [Required]
    [MaxLength(20)]
    public string Identifier { get; set; } = default!;
    
    [Required]
    public DateTime InboundDate { get; set; }
    
    [Required]
    public uint Inventory { get; set; }
    
    [Required]
    public uint Borrowed { get; set; }

    public ICollection<CurrentBorrow> CurrentBorrows { get; set; } = default!;
    
    public ICollection<BorrowHistory> BorrowHistories { get; set; } = default!;
}
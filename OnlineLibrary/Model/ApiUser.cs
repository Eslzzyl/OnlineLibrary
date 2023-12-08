using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Model;

using Microsoft.AspNetCore.Identity;

public class ApiUser : IdentityUser
{
    [Required]
    [MaxLength(150)]
    public string Avatar { get; set; } = null!;
    
    public ICollection<CurrentBorrow> CurrentBorrows { get; set; } = null!;

    public ICollection<BorrowHistory> BorrowHistories { get; set; } = null!;
}
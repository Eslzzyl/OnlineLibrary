using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Model;

public class ApiUser : IdentityUser
{
    [MaxLength(150)]
    public string? Avatar { get; set; }
    
    public ICollection<CurrentBorrow> CurrentBorrows { get; set; } = null!;

    public ICollection<BorrowHistory> BorrowHistories { get; set; } = null!;
}
namespace OnlineLibrary.Model;

using Microsoft.AspNetCore.Identity;

public class ApiUser : IdentityUser
{
    public ICollection<CurrentBorrow> CurrentBorrows { get; set; } = null!;

    public ICollection<BorrowHistory> BorrowHistories { get; set; } = null!;
}
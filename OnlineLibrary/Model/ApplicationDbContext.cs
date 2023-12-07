using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OnlineLibrary.Model;

public class ApplicationDbContext : IdentityDbContext<ApiUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CurrentBorrow>()
            .HasKey(i => new { i.BookId, i.UserId });

        modelBuilder.Entity<CurrentBorrow>()
            .HasOne(x => x.Book)
            .WithMany(y => y.CurrentBorrows)
            .HasForeignKey(x => x.BookId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<CurrentBorrow>()
            .HasOne(x => x.User)
            .WithMany(y => y.CurrentBorrows)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<BorrowHistory>()
            .HasKey(i => new { i.BookId, i.UserId });
        
        modelBuilder.Entity<BorrowHistory>()
            .HasOne(x => x.Book)
            .WithMany(y => y.BorrowHistories)
            .HasForeignKey(x => x.BookId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BorrowHistory>()
            .HasOne(x => x.User)
            .WithMany(y => y.BorrowHistories)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
    
    public DbSet<Book> Books => Set<Book>();
    
    // public DbSet<ApiUser> Users => Set<ApiUser>();

    public DbSet<CurrentBorrow> CurrentBorrows => Set<CurrentBorrow>();

    public DbSet<BorrowHistory> BorrowHistories => Set<BorrowHistory>();
}
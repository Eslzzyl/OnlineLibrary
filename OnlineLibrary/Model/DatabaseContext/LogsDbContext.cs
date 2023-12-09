using Microsoft.EntityFrameworkCore;

namespace OnlineLibrary.Model.DatabaseContext;

public class LogsDbContext : DbContext
{
    public LogsDbContext(DbContextOptions<LogsDbContext> options)
        : base(options) { }

    public DbSet<LogEvent> LogEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<LogEvent>(entity => {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Level).HasColumnType("VARCHAR(10)");
        });
    }
}
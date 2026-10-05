using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>().HasData(
            new Event { Id = 1, Name = "Yankees vs. Red Sox", Venue = "Yankee Stadium",
                Date = new DateTime(2026, 10, 18, 0, 0, 0, DateTimeKind.Utc), LowestPrice = 89.00m },
            new Event { Id = 2, Name = "Knicks vs. Celtics", Venue = "Madison Square Garden",
                Date = new DateTime(2026, 11, 14, 0, 0, 0, DateTimeKind.Utc), LowestPrice = 185.00m },
            new Event { Id = 3, Name = "Hamilton", Venue = "Richard Rodgers Theatre",
                Date = new DateTime(2026, 12, 5, 0, 0, 0, DateTimeKind.Utc), LowestPrice = 249.00m }
        );
    }
}
using Microsoft.EntityFrameworkCore;

public class ItemDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }

    public ItemDbContext(DbContextOptions<ItemDbContext> options)
            : base(options) { }
}

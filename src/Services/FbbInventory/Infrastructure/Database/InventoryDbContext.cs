namespace Bazaar.FbbInventory.Infrastructure.Database;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SellerInventory>()
            .HasIndex(x => x.SellerId)
            .IsUnique();

        modelBuilder.Entity<ProductInventory>(p =>
        {
            p.HasIndex(p => p.ProductId)
            .IsUnique();
        });
    }

    public DbSet<SellerInventory> SellerInventories { get; set; }
    public DbSet<ProductInventory> ProductInventories { get; set; }
}

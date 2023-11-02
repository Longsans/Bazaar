namespace Bazaar.Inventory.Infrastructure.Repositories;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SellerInventory>()
            .HasIndex(x => x.SellerId)
            .IsUnique();

        modelBuilder.Entity<ProductInventory>()
            .HasIndex(x => x.ProductId)
            .IsUnique();
    }

    public DbSet<SellerInventory> SellerInventories { get; set; }
    public DbSet<ProductInventory> ProductInventories { get; set; }
}

namespace Bazaar.Transport.Infrastructure.Repositories;

public class TransportDbContext : DbContext
{
    public TransportDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryPickup>()
            .HasMany(x => x.ProductInventories)
            .WithOne(p => p.Pickup)
            .HasForeignKey(p => p.PickupId);

        modelBuilder.Entity<InventoryReturn>()
            .HasMany(x => x.ReturnQuantities)
            .WithOne(r => r.Return)
            .HasForeignKey(r => r.ReturnId);
    }

    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryPackageItem> DeliveryPackageItems { get; set; }

    public DbSet<InventoryPickup> InventoryPickups { get; set; }
    public DbSet<ProductInventory> InventoryItems { get; set; }

    public DbSet<InventoryReturn> InventoryReturns { get; set; }
    public DbSet<ReturnQuantity> ReturnQuantities { get; set; }
}

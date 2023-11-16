﻿namespace Bazaar.Transport.Infrastructure.Repositories;

public class TransportDbContext : DbContext
{
    public TransportDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryPickup>()
            .HasMany(x => x.ProductInventories)
            .WithOne(x => x.Pickup)
            .HasForeignKey(x => x.PickupId);
    }

    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryPackageItem> DeliveryPackageItems { get; set; }

    public DbSet<InventoryPickup> InventoryPickups { get; set; }
    public DbSet<ProductInventory> InventoryItems { get; set; }
}
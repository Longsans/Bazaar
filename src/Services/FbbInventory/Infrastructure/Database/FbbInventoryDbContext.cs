namespace Bazaar.FbbInventory.Infrastructure.Database;

public class FbbInventoryDbContext : DbContext
{
    public FbbInventoryDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SellerInventory>()
            .HasIndex(x => x.SellerId)
            .IsUnique();

        modelBuilder.Entity<ProductInventory>(inventory =>
        {
            inventory.HasMany(x => x.Lots)
                .WithOne(l => l.ProductInventory)
                .HasForeignKey(l => l.ProductInventoryId);

            inventory.HasIndex(x => x.ProductId)
                .IsUnique();

            inventory.Ignore(x => x.FulfillableLots)
                .Ignore(x => x.UnfulfillableLots)
                .Ignore(x => x.StrandedLots);
        });

        modelBuilder.Entity<Lot>(lot =>
        {
            lot.HasIndex(x => new
            {
                x.ProductInventoryId,
                x.DateUnitsEnteredStorage,
                x.DateUnitsBecameStranded,
                x.DateUnitsBecameUnfulfillable,
            }).IsUnique();

            lot.HasIndex(x => x.LotNumber)
                .IsUnique();

            lot.Property(x => x.LotNumber)
                .HasComputedColumnSql($"CONCAT('{StoragePolicy.LotPrefix}', [{nameof(Lot.Id)}])");
        });
    }

    public DbSet<SellerInventory> SellerInventories { get; set; }
    public DbSet<ProductInventory> ProductInventories { get; set; }
    public DbSet<Lot> Lots { get; set; }
}

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
                .Ignore(x => x.UnfulfillableLots);
        });

        modelBuilder.Entity<Lot>(lot =>
        {
            lot.HasIndex(x => new
            {
                x.ProductInventoryId,
                x.TimeEnteredStorage,
                x.TimeUnfulfillableSince,
                x.UnfulfillableCategory,
            })
            .IsUnique();

            lot.Property(x => x.LotNumber)
            .HasComputedColumnSql(
                $"CONCAT('{StoragePolicy.LotCodePrefix}', [Id])", true);

            lot.ToTable(l => l.HasCheckConstraint("CK_TimeUnfulfillableSince_After_TimeEnteredStorage",
                "[TimeUnfulfillableSince] IS NULL OR [TimeUnfulfillableSince] >= [TimeEnteredStorage]"));
        });
    }

    public DbSet<SellerInventory> SellerInventories { get; set; }
    public DbSet<ProductInventory> ProductInventories { get; set; }
    public DbSet<Lot> Lots { get; set; }

    class Discriminator
    {
        public const string ColumnName = "Fulfillability";
        public const string FulfillableValue = "Fulfillable";
        public const string UnfulfillableValue = "Unfulfillable";
    }
}

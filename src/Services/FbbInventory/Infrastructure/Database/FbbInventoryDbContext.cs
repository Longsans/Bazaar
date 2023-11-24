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
            inventory.HasIndex(x => x.ProductId)
                .IsUnique();
        });

        modelBuilder.Entity<Lot>(lot =>
        {
            lot.HasOne(x => x.ProductInventory)
            .WithMany()
            .HasForeignKey(x => x.ProductInventoryId)
            .OnDelete(DeleteBehavior.Cascade);

            lot.HasDiscriminator<string>(Discriminator.ColumnName)
                .HasValue<FulfillableLot>(Discriminator.FulfillableValue)
                .HasValue<UnfulfillableLot>(Discriminator.UnfulfillableValue);

            lot.Property(x => x.LotNumber)
            .HasComputedColumnSql(
                $"CONCAT(IIF([{Discriminator.ColumnName}] = '{Discriminator.FulfillableValue}', " +
                $"'{StoragePolicy.FulfillableLotCodePrefix}', " +
                $"'{StoragePolicy.UnfulfillableLotCodePrefix}'), [Id])", true);
        });

        modelBuilder.Entity<FulfillableLot>(ffLot =>
        {
            ffLot.HasIndex(x => new
            {
                x.ProductInventoryId,
                x.DateEnteredStorage,
            })
            .IsUnique();

            ffLot.HasOne(x => x.ProductInventory)
            .WithMany()
            .HasForeignKey(x => x.ProductInventoryId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UnfulfillableLot>(ufLot =>
        {
            ufLot.HasIndex(x => new
            {
                x.ProductInventoryId,
                x.DateUnfulfillableSince,
                x.UnfulfillableCategory,
            })
            .IsUnique();

            ufLot.HasOne(x => x.ProductInventory)
            .WithMany()
            .HasForeignKey(x => x.ProductInventoryId)
            .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public DbSet<SellerInventory> SellerInventories { get; set; }
    public DbSet<ProductInventory> ProductInventories { get; set; }
    public DbSet<FulfillableLot> FulfillableLots { get; set; }
    public DbSet<UnfulfillableLot> UnfulfillableLots { get; set; }
    public DbSet<Lot> Lots { get; set; }

    class Discriminator
    {
        public const string ColumnName = "Fulfillability";
        public const string FulfillableValue = "Fulfillable";
        public const string UnfulfillableValue = "Unfulfillable";
    }
}

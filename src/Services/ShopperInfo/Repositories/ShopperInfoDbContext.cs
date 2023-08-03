namespace Bazaar.ShopperInfo.Repositories;

public class ShopperInfoDbContext : DbContext
{
    public ShopperInfoDbContext(DbContextOptions<ShopperInfoDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Shopper>()
            .Property(s => s.ExternalId)
            .HasComputedColumnSql("CONCAT('SPER-', [Id])", stored: true)
            .HasColumnName("ExternalId");
    }

    public DbSet<Shopper> Shoppers { get; set; }
}

namespace Bazaar.ShopperInfo.Infrastructure.Database;

public class ShopperInfoDbContext : DbContext
{
    public ShopperInfoDbContext(DbContextOptions<ShopperInfoDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Shopper>(s =>
        {
            s.Property(x => x.ExternalId)
            .HasComputedColumnSql("CONCAT('SPER-', [Id])", stored: true)
            .HasColumnName("ExternalId");

            s.HasIndex(x => x.EmailAddress)
            .IsUnique();
        });
    }

    public DbSet<Shopper> Shoppers { get; set; }
}

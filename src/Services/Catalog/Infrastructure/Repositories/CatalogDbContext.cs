namespace Bazaar.Catalog.Infrastructure.Repositories;

public class CatalogDbContext : DbContext
{
    public DbSet<CatalogItem> CatalogItems { get; set; }

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CatalogItem>(item =>
        {
            item.Property(x => x.ProductDescription)
                .IsRequired(false);

            item.Property(x => x.ProductId)
                .HasComputedColumnSql("CONCAT('PROD-', [Id])", stored: true)
                .HasColumnName("ProductId");

            item.HasIndex(x => x.ProductId)
                .IsUnique();
        });
    }
}

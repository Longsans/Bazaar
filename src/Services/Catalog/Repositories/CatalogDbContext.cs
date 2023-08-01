namespace Bazaar.Catalog.Repositories
{
    public class CatalogDbContext : DbContext
    {
        public DbSet<CatalogItem> CatalogItems { get; set; }

        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CatalogItem>()
                .Property(x => x.Description)
                .IsRequired(false);

            modelBuilder.Entity<CatalogItem>()
                .Property(x => x.ProductId)
                .HasField("_productId");

            modelBuilder.Entity<CatalogItem>()
                .Property(x => x.ProductId)
                .HasComputedColumnSql("CONCAT('PROD-', [Id])", stored: true)
                .HasColumnName("ProductId");

            modelBuilder.Entity<CatalogItem>()
                .HasIndex(x => x.ProductId)
                .IsUnique();
        }
    }
}

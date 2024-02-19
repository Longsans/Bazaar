namespace Bazaar.Catalog.Infrastructure.Database;

public class CatalogDbContext : DbContext
{
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }

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

            item.HasOne(x => x.MainDepartment)
                .WithMany(c => c.MainDepartmentProducts)
                .HasForeignKey(x => x.MainDepartmentId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            item.HasOne(x => x.Subcategory)
                .WithMany(c => c.SubcategoryProducts)
                .HasForeignKey(x => x.SubcategoryId)
                .IsRequired();
        });

        modelBuilder.Entity<ProductCategory>(c =>
        {
            c.HasOne(x => x.ParentCategory)
                .WithMany(p => p.ChildCategories)
                .IsRequired(false);

            c.HasIndex(x => x.Name)
                .IsUnique();
        });
    }
}

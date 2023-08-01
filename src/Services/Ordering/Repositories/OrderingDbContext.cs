namespace Bazaar.Ordering.Repositories;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .IsRequired();

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.ExternalId)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .Property(o => o.ExternalId)
            .HasComputedColumnSql("CONCAT('ORDR-', [Id])", stored: true)
            .HasColumnName("ExternalId");

        modelBuilder.Entity<OrderItem>()
            .HasIndex(i => new { i.ProductId, i.OrderId })
            .IsUnique();
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}

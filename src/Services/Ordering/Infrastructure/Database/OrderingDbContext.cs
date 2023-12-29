namespace Bazaar.Ordering.Infrastructure.Database;

public class OrderingDbContext : DbContext
{
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(order =>
        {
            order.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .IsRequired();

            order.Property(o => o.Total)
                .HasDefaultValue(0);
        });

        modelBuilder.Entity<OrderItem>()
            .HasIndex(i => new { i.ProductId, i.OrderId })
            .IsUnique();
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}

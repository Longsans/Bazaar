namespace Bazaar.Disposal.Infrastructure.Database;

public class DisposalDbContext(
    DbContextOptions<DisposalDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DisposalOrder>()
            .HasMany(x => x.DisposeQuantities)
            .WithOne(x => x.DisposalOrder)
            .IsRequired();

        modelBuilder.Entity<DisposeQuantity>()
            .HasIndex(x => new { x.LotNumber, x.DisposalOrderId })
            .IsUnique();
    }

    public DbSet<DisposalOrder> DisposalOrders { get; set; }
    public DbSet<DisposeQuantity> DisposeQuantities { get; set; }
}

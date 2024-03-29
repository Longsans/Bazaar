﻿namespace Bazaar.Basket.Infrastructure.Database;

public class BasketDbContext : DbContext
{
    public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BuyerBasket>(basket =>
        {
            basket.HasIndex(b => b.BuyerId)
                .IsUnique();

            basket.HasMany(b => b.Items)
                .WithOne(i => i.Basket)
                .HasForeignKey(i => i.BasketId)
                .HasPrincipalKey(b => b.Id)
                .IsRequired();
        });

        modelBuilder.Entity<BasketItem>()
            .HasIndex(i => new { i.BasketId, i.ProductId })
            .IsUnique();
    }

    public DbSet<BuyerBasket> BuyerBaskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
}
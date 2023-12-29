namespace Bazaar.Basket.Infrastructure.Database;

public static class BasketDbSeeding
{
    private const string BASKETS_SECTION = "baskets";

    public static async Task Seed(this BasketDbContext context, IServiceProvider sp)
    {
        var adapter = sp.GetRequiredService<JsonDataAdapter>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        if (context.BuyerBaskets.Any())
        {
            return;
        }

        context.BuyerBaskets.AddRange(adapter.ReadToObjects<BuyerBasket>(BASKETS_SECTION));
        context.SaveChanges();
    }
}

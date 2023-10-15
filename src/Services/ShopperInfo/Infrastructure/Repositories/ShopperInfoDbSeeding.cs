namespace Bazaar.ShopperInfo.Infrastructure.Repositories;

public static class ShopperInfoDbSeeding
{
    private const string SHOPPERS_SECTION = "shoppers";

    public static async Task Seed(this ShopperInfoDbContext context, IServiceProvider sp)
    {
        await context.Database.MigrateAsync();

        if (context.Shoppers.Any())
        {
            return;
        }

        var adapter = sp.GetRequiredService<JsonDataAdapter>();
        context.Shoppers.AddRange(adapter.ReadToObjects<Shopper>(SHOPPERS_SECTION));
        await context.SaveChangesAsync();
    }
}

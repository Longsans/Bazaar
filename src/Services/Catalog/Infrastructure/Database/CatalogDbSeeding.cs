namespace Bazaar.Catalog.Infrastructure.Database;

public static class CatalogDbSeeding
{
    private const string CATALOG_SECTION = "products";

    public static async Task Seed(this CatalogDbContext context, IServiceProvider sp)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        var adapter = sp.GetRequiredService<JsonDataAdapter>();
        context.CatalogItems.AddRange(
                adapter.ReadToObjects<CatalogItem>(CATALOG_SECTION).ToList());
        context.SaveChanges();
    }
}

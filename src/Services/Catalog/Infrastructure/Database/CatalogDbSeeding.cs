namespace Bazaar.Catalog.Infrastructure.Database;

public static class CatalogDbSeeding
{
    private const string CATALOG_SECTION = "products";

    public static async Task Seed(this CatalogDbContext context, IServiceProvider sp)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        context.ProductCategories.Add(new("Books", null, new List<ProductCategory>
        {
            new("Fantasy books", null, Array.Empty<ProductCategory>()),
            new("Finance books", null, Array.Empty<ProductCategory>()),
        }));
        context.SaveChanges();

        var adapter = sp.GetRequiredService<JsonDataAdapter>();
        context.CatalogItems.AddRange(
                adapter.ReadToObjects<CatalogItem>(CATALOG_SECTION).ToList());
        context.SaveChanges();
    }
}

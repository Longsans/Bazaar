namespace Catalog.Repositories
{
    public static class CatalogDbSeeding
    {
        private const string CATALOG_SECTION = "products";

        public static async Task Seed(this CatalogDbContext context, IServiceProvider sp)
        {
            var adapter = sp.GetRequiredService<JsonDataAdapter>();
            await context.Database.MigrateAsync();

            if (context.CatalogItems.Any())
            {
                return;
            }

            context.CatalogItems.AddRange(
                    adapter.ReadToObjects<CatalogItem>(CATALOG_SECTION).ToList());
            context.SaveChanges();
        }
    }
}

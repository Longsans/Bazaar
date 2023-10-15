namespace Bazaar.Ordering.Infrastructure.Repositories;

public static class OrderingDbSeeding
{
    private const string ORDERS_SECTION = "orders";

    public static async Task Seed(this OrderingDbContext context, IServiceProvider sp)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        var adapter = sp.GetRequiredService<JsonDataAdapter>();
        context.Orders.AddRange(adapter.ReadToObjects<Order>(ORDERS_SECTION));
        context.SaveChanges();
    }
}

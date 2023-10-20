namespace OrderingTests.Helpers;

internal static class DbContextExtensions
{
    public static OrderingDbContext ReseedWithSingleOrder(this OrderingDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var testOrder = new Order("Test address", "SPER-1", new OrderItem[]
            {
                new("PROD-1", "The Winds of Winter", 39.99m, 10, default)
            });
        context.Orders.Add(testOrder);
        context.SaveChanges();

        return context;
    }
}

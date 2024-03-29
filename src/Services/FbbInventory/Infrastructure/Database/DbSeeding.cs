﻿namespace Bazaar.FbbInventory.Infrastructure.Database;

public static class DbSeeding
{
    public static async Task Seed(this FbbInventoryDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        var sellerInventory = new SellerInventory("CLNT-1");
        context.SellerInventories.Add(sellerInventory);
        await context.SaveChangesAsync();

        var prodWidthCm = 16f;
        var prodLengthCm = 24f;
        var prodHeightCm = 5f;
        var productInventory = new ProductInventory(
            "PROD-1", 100, 30, 20,
            10, 2000, prodLengthCm, prodWidthCm, prodHeightCm, sellerInventory.Id);
        context.ProductInventories.Add(productInventory);
        await context.SaveChangesAsync();
    }
}

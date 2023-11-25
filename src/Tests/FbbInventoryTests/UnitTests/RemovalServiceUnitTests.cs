namespace FbbInventoryTests.UnitTests;

public class RemovalServiceUnitTests
{
    #region Test data and helpers
    private static List<ProductInventory> GetTestInventories(uint total, int specialItemsCount,
        Func<int, ProductInventory> specialInitializer)
    {
        var inventories = new List<ProductInventory>();
        for (int i = 1; i < total; i++)
        {
            ProductInventory inventory;
            if (i <= specialItemsCount)
            {
                inventory = specialInitializer(i);
            }
            else
            {
                inventory = new($"PROD-{i}", 100, 5, 10, 10, 1000, 1);
            }
            inventories.Add(inventory);
        }
        return inventories;
    }
    #endregion

    //private readonly Mock<IProductInventoryRepository> _mockInventoryRepo;
    //private readonly RemovalService _service;

    public RemovalServiceUnitTests()
    {
        //_mockInventoryRepo = new();
        //_service = new(_mockInventoryRepo.Object);
    }
}

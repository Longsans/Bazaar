namespace FbbInventoryTests.UnitTests;

public class InventoryDisposalServiceUnitTests
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
                inventory = new($"PROD-{i}", 100, 10, 1000, 1);
            }
            inventories.Add(inventory);
        }
        return inventories;
    }
    #endregion

    private readonly Mock<IProductInventoryRepository> _mockInventoryRepo;
    private readonly InventoryDisposalService _service;

    public InventoryDisposalServiceUnitTests()
    {
        _mockInventoryRepo = new();
        _service = new(_mockInventoryRepo.Object);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void MarkOverdueUnfulfillableInventoryForDisposal_MarksItemsUnfulfillableOverStoragePolicy(
        int unfulfillableCount)
    {
        var inventories = GetTestInventories(3, unfulfillableCount, index =>
        {
            var inventory = new ProductInventory($"PROD-{index}", 0, 10, 1000, 1);
            typeof(ProductInventory).GetProperty(nameof(inventory.UnfulfillableSince))!
                .SetValue(inventory, DateTime.Now.Date - StoragePolicy.MaximumUnfulfillableDuration);
            return inventory;
        });

        _mockInventoryRepo.Setup(x => x.GetAll())
            .Returns(inventories);

        _service.MarkOverdueUnfulfillableInventoriesForDisposal();

        foreach (var (inven, index) in inventories.Select((x, i) => (x, i)))
        {
            if (index < unfulfillableCount)
            {
                Assert.Equal(InventoryStatus.ToBeDisposed, inven.Status);
            }
            else
            {
                Assert.Equal(InventoryStatus.Ready, inven.Status);
            }
        }
    }
}

namespace FbbInventoryTests.UnitTests;

public class DeleteProductInventoryServiceUnitTests
{
    private readonly DeleteProductInventoryService _service;
    private readonly ProductInventory _testInventory;
    private readonly SellerInventory _testSellerInv;

    public DeleteProductInventoryServiceUnitTests(EventBusTestDouble testEventBus)
    {
        var repoMock = new Mock<IRepository<ProductInventory>>();
        _service = new(repoMock.Object, testEventBus);

        _testInventory = new("PROD-1", 100, 5, 10, 10, 1000, 1);
        _testSellerInv = new SellerInventory("CLNT-1");
        _testSellerInv.ProductInventories.Add(_testInventory);
        typeof(ProductInventory).GetProperty(nameof(ProductInventory.SellerInventory))!
            .SetValue(_testInventory, _testSellerInv);

        repoMock.Setup(x => x.SingleOrDefaultAsync(
                It.IsAny<ProductInventoryWithLotsAndSellerSpec>(), CancellationToken.None))
            .Returns(Task.FromResult(_testInventory));
    }

    #region Helpers
    private static void SetProductInventoryHasPickups(ProductInventory productInv, bool hasPickups)
    {
        typeof(ProductInventory).GetProperty(nameof(ProductInventory.HasPickupsInProgress))!
            .SetValue(productInv, hasPickups);
    }

    private static void SetProductInventoryUnits(ProductInventory productInv,
        uint fulfillableUnits, uint unfulfillableUnits)
    {
        var lots = new List<Lot>();
        if (fulfillableUnits > 0)
            lots.Add(new(productInv, fulfillableUnits));
        if (unfulfillableUnits > 0)
            lots.Add(new(productInv, unfulfillableUnits, UnfulfillableCategory.Defective));

        typeof(ProductInventory).GetField("_lots",
                BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(productInv, lots);
    }
    #endregion

    [Fact]
    public async Task DeleteProductInventory_RemovesProductInventoryAndReturnsSuccess_WhenAllValid()
    {
        SetProductInventoryUnits(_testInventory, 0, 0);

        var result = await _service.DeleteProductInventory(_testInventory.ProductId);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(_testInventory, _testSellerInv.ProductInventories);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    public async Task DeleteProductInventory_ReturnsConflict_WhenProductInventoryStillHasUnits(
        uint ffUnits, uint ufUnits)
    {
        SetProductInventoryUnits(_testInventory, ffUnits, ufUnits);

        var result = await _service.DeleteProductInventory(_testInventory.ProductId);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains(_testInventory, _testSellerInv.ProductInventories);
    }

    [Fact]
    public async Task DeleteProductInventory_ReturnsConflict_WhenProductInventoryHasPickupsInProgress()
    {
        SetProductInventoryUnits(_testInventory, 0u, 0u);
        SetProductInventoryHasPickups(_testInventory, true);

        var result = await _service.DeleteProductInventory(_testInventory.ProductId);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains(_testInventory, _testSellerInv.ProductInventories);
    }
}

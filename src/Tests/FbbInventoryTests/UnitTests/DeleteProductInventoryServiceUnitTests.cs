namespace FbbInventoryTests.UnitTests;

public class DeleteProductInventoryServiceUnitTests
{
    private readonly DeleteProductInventoryService _service;
    private readonly Mock<IProductInventoryRepository> _productInvRepoMock;

    private readonly ProductInventory _testProductInv;
    private readonly SellerInventory _testSellerInv;

    public DeleteProductInventoryServiceUnitTests(EventBusTestDouble testEventBus)
    {
        _productInvRepoMock = new Mock<IProductInventoryRepository>();
        _service = new(_productInvRepoMock.Object, testEventBus);

        _testProductInv = new("PROD-1", 100, 5, 10, 10, 1000, 1);
        _testSellerInv = new SellerInventory("CLNT-1");
        _testSellerInv.ProductInventories.Add(_testProductInv);
        typeof(ProductInventory).GetProperty(nameof(ProductInventory.SellerInventory))!
            .SetValue(_testProductInv, _testSellerInv);

        SetProductInventoryUnits(_testProductInv, 0, 0);
        _productInvRepoMock.Setup(x => x.GetByProductId(It.IsAny<string>()))
            .Returns(_testProductInv);
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
    public void DeleteProductInventory_RemovesProductInventoryAndReturnsSuccess_WhenAllValid()
    {
        var result = _service.DeleteProductInventory(_testProductInv.ProductId);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(_testProductInv, _testSellerInv.ProductInventories);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    public void DeleteProductInventory_ReturnsConflict_WhenProductInventoryStillHasUnits(
        uint ffUnits, uint ufUnits)
    {
        SetProductInventoryUnits(_testProductInv, ffUnits, ufUnits);

        var result = _service.DeleteProductInventory(_testProductInv.ProductId);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains(_testProductInv, _testSellerInv.ProductInventories);
    }

    [Fact]
    public void DeleteProductInventory_ReturnsConflict_WhenProductInventoryHasPickupsInProgress()
    {
        SetProductInventoryHasPickups(_testProductInv, true);

        var result = _service.DeleteProductInventory(_testProductInv.ProductId);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains(_testProductInv, _testSellerInv.ProductInventories);
    }
}

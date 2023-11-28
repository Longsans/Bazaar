namespace FbbInventoryTests.UnitTests;

public class DeleteProductInventoryServiceUnitTests
{
    private readonly DeleteProductInventoryService _service;
    private readonly Mock<IProductInventoryRepository> _productInvRepoMock;
    private readonly EventBusTestDouble _testEventBus;

    private readonly ProductInventory _testProductInv;
    private readonly SellerInventory _testSellerInv;

    public DeleteProductInventoryServiceUnitTests(EventBusTestDouble testEventBus)
    {
        _testEventBus = testEventBus;
        _productInvRepoMock = new Mock<IProductInventoryRepository>();
        _service = new(_productInvRepoMock.Object, _testEventBus);

        _testProductInv = new("PROD-1", 100, 5, 10, 10, 1000, 1);
        _testSellerInv = new SellerInventory("CLNT-1");
        SetProductInventoryUnits(_testProductInv, 0, 0);
        SetupProductInvWithSellerInv(_testProductInv, _testSellerInv);
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
        var ffLots = fulfillableUnits > 0
            ? new List<FulfillableLot>
            {
                new(productInv, fulfillableUnits)
            }
            : new List<FulfillableLot>();
        var ufLots = unfulfillableUnits > 0
            ? new List<UnfulfillableLot>
            {
                new(productInv, unfulfillableUnits, UnfulfillableCategory.Defective)
            }
            : new List<UnfulfillableLot>();

        typeof(ProductInventory).GetField("_fulfillableLots",
                BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(productInv, ffLots);
        typeof(ProductInventory).GetField("_unfulfillableLots",
                BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(productInv, ufLots);
    }

    private static void SetupProductInvWithSellerInv(
        ProductInventory productInv, SellerInventory sellerInv)
    {
        typeof(ProductInventory).GetProperty(nameof(ProductInventory.SellerInventory))!
            .SetValue(productInv, sellerInv);
        typeof(SellerInventory).GetProperty(nameof(SellerInventory.ProductInventories))!
            .SetValue(sellerInv, new List<ProductInventory> { productInv });
    }

    private static void SetupProductInventoryRepo(
        Mock<IProductInventoryRepository> prodInvRepoMock,
        ProductInventory? getByIdResult = null, ProductInventory? getByProdIdResult = null)
    {
        prodInvRepoMock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns(getByIdResult);

        prodInvRepoMock.Setup(x => x.GetByProductId(It.IsAny<string>()))
            .Returns(getByProdIdResult);
    }
    #endregion

    [Fact]
    public void DeleteProductInventory_RemovesProdInvAndPublishesEvent_WhenAllValid()
    {
        SetupProductInventoryRepo(_productInvRepoMock, _testProductInv);

        var result = _service.DeleteProductInventory(1);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(_testProductInv, _testSellerInv.ProductInventories);

        var publishedEvent = _testEventBus.GetEvent<ProductFbbInventoryDeletedIntegrationEvent>();
        Assert.NotNull(publishedEvent);
        Assert.Equal(_testProductInv.ProductId, publishedEvent.ProductId);
    }

    [Fact]
    public void DeleteProductInventory_ById_DoesNothingAndReturnsSuccess_WhenProductInventoryNotFound()
    {
        SetupProductInventoryRepo(_productInvRepoMock);

        var result = _service.DeleteProductInventory(1);

        Assert.True(result.IsSuccess);
        Assert.Contains(_testProductInv, _testSellerInv.ProductInventories);
    }

    [Fact]
    public void DeleteProductInventory_ByProductId_DoesNothingAndReturnsSuccess_WhenProductInventoryNotFound()
    {
        SetupProductInventoryRepo(_productInvRepoMock);

        var result = _service.DeleteProductInventory("PROD-1");

        Assert.True(result.IsSuccess);
        Assert.Contains(_testProductInv, _testSellerInv.ProductInventories);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    public void DeleteProductInventory_ReturnsConflict_WhenProductInvStillHasUnits(
        uint ffUnits, uint ufUnits)
    {
        SetProductInventoryUnits(_testProductInv, ffUnits, ufUnits);
        SetupProductInventoryRepo(_productInvRepoMock, _testProductInv);

        var result = _service.DeleteProductInventory(1);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains(_testProductInv, _testSellerInv.ProductInventories);
    }

    [Fact]
    public void DeleteProductInventory_ReturnsConflict_WhenProductInvHasPickupsInProgress()
    {
        SetProductInventoryHasPickups(_testProductInv, true);
        SetupProductInventoryRepo(_productInvRepoMock, _testProductInv);

        var result = _service.DeleteProductInventory(1);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Contains(_testProductInv, _testSellerInv.ProductInventories);
    }
}

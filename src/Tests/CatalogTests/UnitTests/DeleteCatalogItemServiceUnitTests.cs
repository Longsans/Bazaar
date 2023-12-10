namespace CatalogTests.UnitTests;

public class DeleteCatalogItemServiceUnitTests
{
    #region Test data and helpers
    private CatalogItem? _testItem;

    private void InitializeTestCatalogItem(uint stock, FulfillmentMethod ffMethod)
    {
        uint modifiedStock = ffMethod == FulfillmentMethod.Fbb ? 0 : stock;
        _testItem = new CatalogItem("The Winds of Winter",
            "George is not gonna finish this.", 39.99m, modifiedStock, "CLNT-1", ffMethod);

        if (_testItem.IsFbb && stock != 0u)
            _testItem.Restock(stock);
    }
    #endregion

    private readonly EventBusTestDouble _testEventBus;
    private readonly ICatalogRepository _testCatalogRepo;
    private readonly Mock<ICatalogRepository> _repoMock;
    private readonly DeleteCatalogItemService _service;

    public DeleteCatalogItemServiceUnitTests(EventBusTestDouble testEventBus)
    {
        _repoMock = new Mock<ICatalogRepository>();
        _testEventBus = testEventBus;
        _testCatalogRepo = _repoMock.Object;
        _service = new DeleteCatalogItemService(_testCatalogRepo, _testEventBus);
    }

    [Theory]
    [InlineData(FulfillmentMethod.Merchant, 100)]
    [InlineData(FulfillmentMethod.Fbb, 0)]
    [InlineData(FulfillmentMethod.Merchant, 0)]
    public void AssertCanBeDeleted_Succeeds_WhenValid(FulfillmentMethod ffMethod, uint stock)
    {
        InitializeTestCatalogItem(stock, ffMethod);

        _service.AssertCanBeDeleted(_testItem!);
    }

    [Fact]
    public void AssertCanBeDeleted_ThrowsException_WhenProductHasOrdersInProgress()
    {
        InitializeTestCatalogItem(100, FulfillmentMethod.Merchant);
        typeof(CatalogItem).GetProperty(nameof(_testItem.HasOrdersInProgress))!
            .SetValue(_testItem, true);

        Assert.Throws<DeleteProductWithOrdersInProgressException>(() =>
        {
            _service.AssertCanBeDeleted(_testItem!);
        });
    }

    [Fact]
    public void AssertCanBeDeleted_ThrowsException_WhenProductFbbInventoryNotEmpty()
    {
        InitializeTestCatalogItem(100, FulfillmentMethod.Fbb);

        Assert.Throws<DeleteFbbProductWhenFbbInventoryNotEmptyException>(() =>
        {
            _service.AssertCanBeDeleted(_testItem!);
        });
    }

    [Fact]
    public void SoftDeleteById_Succeeds_WhenValid()
    {
        InitializeTestCatalogItem(100, FulfillmentMethod.Merchant);
        _repoMock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns(_testItem);

        _service.SoftDeleteById(_testItem!.Id);

        Assert.True(_testItem.IsDeleted);
        Assert.Equal(0u, _testItem.AvailableStock);
    }
}

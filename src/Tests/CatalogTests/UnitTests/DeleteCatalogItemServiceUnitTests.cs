using Bazaar.Catalog.Domain.Exceptions;

namespace CatalogTests.UnitTests;

public class DeleteCatalogItemServiceUnitTests
{
    #region Test data and helpers
    private CatalogItem? _testItem;

    private void InitializeTestCatalogItem(uint stock, bool isFbb)
    {
        _testItem = new CatalogItem(1, "PROD-1", "The Winds of Winter",
            "George is not gonna finish this.", 39.99m, stock, "CLNT-1", isFbb);
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
    [InlineData(false, 100)]
    [InlineData(true, 0)]
    [InlineData(false, 0)]
    public void AssertCanBeDeleted_Succeeds_WhenValid(bool isFbb, uint stock)
    {
        InitializeTestCatalogItem(100, isFbb);
        if (stock != _testItem!.AvailableStock)
        {
            _testItem.ReduceStock(_testItem.AvailableStock - stock);
        }

        _service.AssertCanBeDeleted(_testItem!);
    }

    [Fact]
    public void AssertCanBeDeleted_ThrowsException_WhenProductHasOrdersInProgress()
    {
        InitializeTestCatalogItem(100, false);
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
        InitializeTestCatalogItem(100, true);

        Assert.Throws<DeleteFbbProductWhenFbbInventoryNotEmptyException>(() =>
        {
            _service.AssertCanBeDeleted(_testItem!);
        });
    }

    [Fact]
    public void SoftDeleteById_Succeeds_WhenValid()
    {
        InitializeTestCatalogItem(100, false);
        _repoMock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns(_testItem);

        _service.SoftDeleteById(_testItem!.Id);

        Assert.True(_testItem.IsDeleted);
        Assert.Equal(0u, _testItem.AvailableStock);
    }
}

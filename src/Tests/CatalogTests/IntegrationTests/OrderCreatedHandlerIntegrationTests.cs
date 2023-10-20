namespace CatalogTests.IntegrationTests;

public class OrderCreatedHandlerIntegrationTests
{
    private readonly OrderCreatedIntegrationEventHandler _handler;
    private readonly CatalogItem _testCatalogItem;

    private readonly EventBusTestDouble _testEventBus;
    private readonly ICatalogRepository _repo;

    public OrderCreatedHandlerIntegrationTests(CatalogDbContext dbContext, EventBusTestDouble testEventBus)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        _testCatalogItem = new CatalogItem(default, string.Empty,
            "Test", "Test description", 19.99m,
            100, "PNER-1", 10, 500);

        dbContext.CatalogItems.Add(_testCatalogItem);
        dbContext.SaveChanges();

        _repo = new CatalogRepository(dbContext);
        _testEventBus = testEventBus;
        _handler = new OrderCreatedIntegrationEventHandler(_repo, _testEventBus);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(100)]
    public async Task Handle_UpdatesItemStocks_When_StocksAdequate(uint orderedUnits)
    {
        // arrange
        uint remainingUnits = _testCatalogItem.AvailableStock - orderedUnits;
        var orderCreatedEvent = new OrderCreatedIntegrationEvent(1, new OrderStockItem[]
        {
            new("PROD-1", orderedUnits)
        });

        // act
        await _handler.Handle(orderCreatedEvent);

        // assert
        Assert.Equal(remainingUnits, _testCatalogItem.AvailableStock);

        var publishedEvent = _testEventBus.GetEvent<OrderStocksConfirmedIntegrationEvent>();
        Assert.NotNull(publishedEvent);
        Assert.Equal(orderCreatedEvent.OrderId, publishedEvent.OrderId);
    }

    [Fact]
    public async Task Handle_PublishesItemUnavailableEvent_When_ItemsUnavailable()
    {
        // arrange
        uint orderedUnits = 20;
        uint originalStock = _testCatalogItem.AvailableStock;
        var orderCreatedEvent = new OrderCreatedIntegrationEvent(1, new OrderStockItem[]
        {
            new("PROD-1000", orderedUnits)
        });

        // act
        await _handler.Handle(orderCreatedEvent);

        // assert
        Assert.Equal(originalStock, _testCatalogItem.AvailableStock);

        var publishedEvent = _testEventBus.GetEvent<OrderItemsUnavailableIntegrationEvent>();
        Assert.NotNull(publishedEvent);
        Assert.Equal(orderCreatedEvent.OrderId, publishedEvent.OrderId);
    }

    [Fact]
    public async Task Handle_PublishesStockInadequateEvent_When_StocksInadequate()
    {
        // arrange
        uint orderedUnits = 2000;
        uint originalStock = _testCatalogItem.AvailableStock;
        var orderCreatedEvent = new OrderCreatedIntegrationEvent(1, new OrderStockItem[]
        {
            new("PROD-1", orderedUnits)
        });

        // act
        await _handler.Handle(orderCreatedEvent);

        // assert
        Assert.Equal(originalStock, _testCatalogItem.AvailableStock);

        var publishedEvent = _testEventBus.GetEvent<OrderStocksInadequateIntegrationEvent>();
        Assert.NotNull(publishedEvent);
        Assert.Equal(orderCreatedEvent.OrderId, publishedEvent.OrderId);
    }

    [Fact]
    public async Task Handle_DoesNotChangeStockNorPublishEvent_When_StocksNumberInvalid()
    {
        // arrange
        uint orderedUnits = 0;
        uint originalStock = _testCatalogItem.AvailableStock;
        var orderCreatedEvent = new OrderCreatedIntegrationEvent(1, new OrderStockItem[]
        {
            new("PROD-1", orderedUnits)
        });

        // act
        await _handler.Handle(orderCreatedEvent);

        // assert
        Assert.Equal(originalStock, _testCatalogItem.AvailableStock);
        Assert.False(_testEventBus.PublishedAnyEvent);
    }
}
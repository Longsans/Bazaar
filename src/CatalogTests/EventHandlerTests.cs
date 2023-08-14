namespace Bazaar.CatalogTests;

public class EventHandlerTests
{
    private readonly CatalogDbContext _dbContext;

    public EventHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
                        .UseSqlServer("Server=localhost\\MSSQLSERVER01;Database=Bazaar;Trusted_Connection=True;TrustServerCertificate=True;")
                        .Options;
        _dbContext = new CatalogDbContext(options);
    }

    [Fact]
    public async Task OrderCreatedEventHandler_Updates_ItemStocks_When_StocksAdequate()
    {
        // arrange
        var items = GetItems(("PROD-1", 20), ("PROD-2", 20));
        var (handler, eventBusMock, orderCreatedEvent) = GetArrangedObjects(items);

        // act
        await handler.Handle(orderCreatedEvent);

        // assert
        Assert.Equal(19, items[0].AvailableStock);
        Assert.Equal(18, items[1].AvailableStock);
        eventBusMock.Verify(eb => eb.Publish(It.IsAny<OrderItemsUnavailableIntegrationEvent>()), Times.Never);
        eventBusMock.Verify(eb => eb.Publish(It.IsAny<OrderStocksInadequateIntegrationEvent>()), Times.Never);
    }

    [Fact]
    public async Task OrderCreateEventHandler_Publishes_ItemUnavailableEvent_When_ItemsUnavailable()
    {
        // arrange
        var items = GetItems(("PROD-1", 20));
        var (handler, eventBusMock, orderCreatedEvent) = GetArrangedObjects(items);

        // act
        await handler.Handle(orderCreatedEvent);

        // assert
        var product = _dbContext.CatalogItems.FirstOrDefault(item => item.ProductId == "PROD-1")!;

        Assert.Equal(20, product.AvailableStock);
        eventBusMock.Verify(eb => eb.Publish(It.IsAny<OrderItemsUnavailableIntegrationEvent>()), Times.Once);
        eventBusMock.Verify(eb => eb.Publish(It.IsAny<OrderStocksInadequateIntegrationEvent>()), Times.Never);
    }

    [Fact]
    public async Task OrderCreateEventHandler_Publishes_StockInadequateEvent_When_StocksInadequate()
    {
        // arrange
        var items = GetItems(("PROD-1", 0), ("PROD-2", 0));
        var (handler, eventBusMock, orderCreatedEvent) = GetArrangedObjects(items);

        // act
        await handler.Handle(orderCreatedEvent);

        // assert
        Assert.Equal(0, items[0].AvailableStock);
        Assert.Equal(0, items[1].AvailableStock);
        eventBusMock.Verify(eb => eb.Publish(It.IsAny<OrderItemsUnavailableIntegrationEvent>()), Times.Never);
        eventBusMock.Verify(eb => eb.Publish(It.IsAny<OrderStocksInadequateIntegrationEvent>()), Times.Once);
    }

    private static CatalogItem[] GetItems(params (string, int)[] products)
    {
        return products.Select(p => new CatalogItem
        {
            ProductId = p.Item1,
            AvailableStock = p.Item2,
            Name = "Name",
            SellerId = "seller"
        }).ToArray();
    }

    private (
        OrderCreatedIntegrationEventHandler,
        Mock<IEventBus>,
        OrderCreatedIntegrationEvent) GetArrangedObjects(CatalogItem[] items)
    {
        _dbContext.CatalogItems.ExecuteDelete();
        _dbContext.Database.ExecuteSqlRaw("DBCC CHECKIDENT([CatalogItems], RESEED, 0);");
        _dbContext.CatalogItems.AddRange(items);
        _dbContext.SaveChanges();

        var eventBusMock = new Mock<IEventBus>();
        eventBusMock
            .Setup(eb => eb.Publish(It.IsAny<OrderItemsUnavailableIntegrationEvent>()))
            .Verifiable();
        eventBusMock
            .Setup(eb => eb.Publish(It.IsAny<OrderStocksInadequateIntegrationEvent>()))
            .Verifiable();

        var eventBus = eventBusMock.Object;

        var orderCreatedEvent = new OrderCreatedIntegrationEvent(1, new[]
        {
            new OrderStockItem("PROD-1", 1),
            new OrderStockItem("PROD-2", 2),
        });

        return (
            new OrderCreatedIntegrationEventHandler(_dbContext, eventBus),
            eventBusMock,
            orderCreatedEvent);
    }
}
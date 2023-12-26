using Microsoft.EntityFrameworkCore;

namespace OrderingTests.IntegrationTests;

[Collection("OrderingIntegrationTests")]
[CollectionDefinition("OrderingIntegrationTests", DisableParallelization = true)]
public class BasketCheckoutAcceptedHandlerIntegrationTests : IDisposable
{
    private readonly BasketCheckoutAcceptedIntegrationEventHandler _handler;
    private readonly IOrderRepository _orderRepo;
    private readonly EventBusTestDouble _testEventBus;
    private readonly OrderingDbContext _dbContext;

    #region Test data and helpers
    private BasketCheckoutAcceptedIntegrationEvent _testEvent;

    private Order? GetOnlyOrderInDb()
    {
        return _dbContext.Orders
            .Include(x => x.Items)
            .SingleOrDefault();
    }

    public BasketCheckoutAcceptedHandlerIntegrationTests(
        OrderingDbContext dbContext, EventBusTestDouble testEventBus)
    {
        _testEventBus = testEventBus;
        _dbContext = dbContext;

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _orderRepo = new OrderRepository(_dbContext);
        var handleService = new HandleOrderService(_orderRepo, _testEventBus);
        _handler = new BasketCheckoutAcceptedIntegrationEventHandler(handleService);

        _testEvent = new BasketCheckoutAcceptedIntegrationEvent(
            "SPER-1",
            "Albuquerque, N.M",
            "United States",
            "87101",
            "308 Negra Arroyo Lane",
            new CheckoutEventBasketItem[]
            {
                new("PROD-1", "The Winds of Winter", 39.99m, 10),
            });
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
    }
    #endregion

    [Fact]
    public async Task Handle_CreatesOrderAndPublishesEvent_WhenBasketHasItems()
    {
        // act
        await _handler.Handle(_testEvent);

        // assert
        var createdOrder = GetOnlyOrderInDb();
        Assert.NotNull(createdOrder);
        Assert.Equal(_testEvent.BuyerId, createdOrder.BuyerId);

        var shippingAddressJoined = string.Join(", ", _testEvent.ShippingAddress,
            _testEvent.City, _testEvent.Country);
        Assert.Equal(shippingAddressJoined, createdOrder.ShippingAddress);

        foreach (var (basketItem, index) in _testEvent.BasketItems
            .Select((item, index) => (item, index)))
        {
            var orderItem = createdOrder.Items.ElementAt(index);
            Assert.Equal(basketItem.ProductId, orderItem.ProductId);
            Assert.Equal(basketItem.ProductUnitPrice, orderItem.ProductUnitPrice);
            Assert.Equal(basketItem.Quantity, orderItem.Quantity);
        }

        var publishedEvent = _testEventBus.GetEvent<OrderPlacedIntegrationEvent>();
        Assert.NotNull(publishedEvent);
        Assert.Equal(createdOrder.Id, publishedEvent.OrderId);
    }

    [Fact]
    public async Task Handle_DoesNotCreateOrderNorPublishesEvent_WhenBasketEmpty()
    {
        // arrange
        _testEvent = new BasketCheckoutAcceptedIntegrationEvent(
            "SPER-1",
            "Albuquerque, N.M",
            "United States",
            "87101",
            "308 Negra Arroyo Lane",
            Array.Empty<CheckoutEventBasketItem>());

        // act
        await _handler.Handle(_testEvent);

        // assert
        var order = GetOnlyOrderInDb();
        Assert.Null(order);

        var publishedEvent = _testEventBus.GetEvent<OrderPlacedIntegrationEvent>();
        Assert.Null(publishedEvent);
    }
}

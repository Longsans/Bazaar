using Microsoft.EntityFrameworkCore;

namespace OrderingTests.IntegrationTests;

[Collection("OrderingIntegrationTests")]
[CollectionDefinition("OrderingIntegrationTests", DisableParallelization = true)]
public class BuyerCheckoutAcceptedHandlerIntegrationTests : IDisposable
{
    private readonly BasketCheckoutAcceptedIntegrationEventHandler _handler;
    private readonly IOrderRepository _orderRepo;
    private readonly EventBusTestDouble _testEventBus;
    private readonly OrderingDbContext _dbContext;

    #region Test data and helpers
    private readonly BasketCheckoutAcceptedIntegrationEvent _testEvent;

    private Order? GetOnlyOrderInDb()
    {
        return _dbContext.Orders
            .Include(x => x.Items)
            .SingleOrDefault();
    }

    public BuyerCheckoutAcceptedHandlerIntegrationTests(
        OrderingDbContext dbContext, EventBusTestDouble testEventBus)
    {
        _testEventBus = testEventBus;
        _dbContext = dbContext;

        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _orderRepo = new OrderRepository(_dbContext);
        _handler = new BasketCheckoutAcceptedIntegrationEventHandler(
            _orderRepo, _testEventBus);

        _testEvent = new BuyerCheckoutAcceptedIntegrationEvent()
        {
            BuyerId = "SPER-1",
            City = "Albuquerque, N.M",
            Country = "United States",
            ZipCode = "87101",
            ShippingAddress = "308 Negra Arroyo Lane",

            CardNumber = "1",
            CardHolderName = "Walter White",
            CardExpiration = DateTime.Now.AddYears(5),
            CardSecurityNumber = "1000",

            Basket = new("SPER-1", new CheckoutEventBasketItem[]
            {
                new("PROD-1", "The Winds of Winter", 39.99m, 10),
            })
        };
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

        foreach (var (eventItem, index) in _testEvent.Basket.Items
            .Select((item, index) => (item, index)))
        {
            var orderItem = createdOrder.Items.ElementAt(index);
            Assert.Equal(eventItem.ProductId, orderItem.ProductId);
            Assert.Equal(eventItem.ProductUnitPrice, orderItem.ProductUnitPrice);
            Assert.Equal(eventItem.Quantity, orderItem.Quantity);
        }

        var publishedEvent = _testEventBus.GetEvent<OrderPlacedIntegrationEvent>();
        Assert.NotNull(publishedEvent);
        Assert.Equal(createdOrder.Id, publishedEvent.OrderId);
    }

    [Fact]
    public async Task Handle_DoesNotCreateOrderNorPublishesEvent_WhenBasketEmpty()
    {
        // arrange
        typeof(CheckoutEventBasket)
            .GetProperty(nameof(_testEvent.Basket.Items))!
            .SetValue(_testEvent.Basket, Array.Empty<CheckoutEventBasketItem>());

        // act
        await _handler.Handle(_testEvent);

        // assert
        var order = GetOnlyOrderInDb();
        Assert.Null(order);

        var publishedEvent = _testEventBus.GetEvent<OrderPlacedIntegrationEvent>();
        Assert.Null(publishedEvent);
    }
}

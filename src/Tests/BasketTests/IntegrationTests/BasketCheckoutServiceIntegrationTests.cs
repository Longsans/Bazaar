﻿namespace BasketTests.IntegrationTests;

public class BasketCheckoutServiceIntegrationTests : IDisposable
{
    private readonly IBasketRepository _basketRepo;
    private readonly EventBusTestDouble _testEventBus;
    private readonly BasketCheckoutService _checkoutService;
    private readonly BuyerBasket _testBasket;
    private readonly BasketDbContext _dbContext;

    private const string _validBuyerId = "PNER-1";
    private const string _newBuyerId = "PNER-1000";
    private const string _shippingAddress = "308 Negra Arroyo Lane";
    private const string _city = "Albuquerque, New Mexico";
    private const string _country = "USA";
    private const string _zipcode = "73000";

    public BasketCheckoutServiceIntegrationTests(
        BasketDbContext dbContext, EventBusTestDouble testEventBus)
    {
        _dbContext = dbContext;
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        _testBasket = new(_validBuyerId);
        _dbContext.BuyerBaskets.Add(_testBasket);
        _dbContext.SaveChanges();

        _basketRepo = new BasketRepository(_dbContext);
        _testEventBus = testEventBus;
        _checkoutService = new BasketCheckoutService(_basketRepo, _testEventBus);
    }

    [Fact]
    public void Checkout_PublishesCheckoutAcceptedEventAndEmptiesBasket_WhenValid()
    {
        // arrange
        var item1 = new BasketItem("PROD-1", "The Winds of Winter", 39.99m, 10,
            "https://imgur.com/give-him-10-more-yrs", _testBasket);
        var item2 = new BasketItem("PROD-2", "A Dream of Spring", 29.99m, 2,
            "https://imgur.com/lol", _testBasket);

        _testBasket.AddItem(item1);
        _testBasket.AddItem(item2);
        _basketRepo.Update(_testBasket);

        var checkout = new BasketCheckout(_testBasket.BuyerId, _shippingAddress, _city, _country, _zipcode);

        // act
        var result = _checkoutService.Checkout(checkout);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Empty(_testBasket.Items);

        var publishedEvent = _testEventBus.GetEvent<BasketCheckoutAcceptedIntegrationEvent>();
        Assert.NotNull(publishedEvent);
        Assert.Equal(_testBasket.BuyerId, publishedEvent.BuyerId);

        var dbBasket = _basketRepo.GetWithItemsByBuyerId(_testBasket.BuyerId);
        Assert.Equal(_testBasket, dbBasket);
    }

    [Theory]
    [InlineData(_validBuyerId)]
    [InlineData(_newBuyerId)]
    public void Checkout_DoesNotPublishEventsNorDoesAnythingWithBasket_WhenBasketHasNoItems(
        string buyerId)
    {
        var checkout = new BasketCheckout(buyerId, _shippingAddress, _city, _country, _zipcode);

        var result = _checkoutService.Checkout(checkout);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        var publishedEvent = _testEventBus.GetEvent<BasketCheckoutAcceptedIntegrationEvent>();
        Assert.Null(publishedEvent);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
    }
}

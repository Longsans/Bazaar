using Bazaar.Basket.Domain.Entites;
using Bazaar.Basket.Domain.Exceptions;

namespace BasketTests.UnitTests;

public class BasketUnitTests
{
    private readonly BuyerBasket _testBasket;
    private readonly BasketItem _testItem;

    private const string _productId = "PROD-2";
    private const string _productName = "A Dream of Spring";
    private const decimal _validProductUnitPrice = 19.99m;
    private const uint _validQuantity = 2;
    private const string _imgUrl = "https://imgur.com/yeahright";

    public BasketUnitTests()
    {
        _testBasket = new("PNER-1");
        _testItem = new("PROD-1", "The Winds of Winter", 39.99m, 10,
            "https://imgur.com/give-him-10-more-yrs", _testBasket);
    }

    [Fact]
    public void BasketItemConstructor_Succeeds_WhenValid()
    {
        var item = new BasketItem(_productId, _productName,
            _validProductUnitPrice, _validQuantity, _imgUrl, _testBasket);

        Assert.Equal(_productId, item.ProductId);
        Assert.Equal(_validProductUnitPrice, item.ProductUnitPrice);
        Assert.Equal(_validQuantity, item.Quantity);
        Assert.Equal(_testBasket, item.Basket);
    }

    [Theory]
    [InlineData(-19.99)]
    [InlineData(0)]
    public void BasketItemConstructor_ThrowsArgOutOfRangeException_WhenUnitPriceNotGreaterThanZero(
        decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var item = new BasketItem(_productId, _productName,
                price, _validQuantity, _imgUrl, _testBasket);
        });
    }

    [Fact]
    public void BasketItemConstructor_ThrowsArgOutOfRangeException_WhenQuantityIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var item = new BasketItem(_productId, _productName,
                _validProductUnitPrice, 0, _imgUrl, _testBasket);
        });
    }

    [Fact]
    public void AddItem_Succeeds_WhenValid()
    {
        _testBasket.AddItem(_testItem);

        Assert.Contains(_testItem, _testBasket.Items);
        Assert.Equal(_testItem.Quantity * _testItem.ProductUnitPrice, _testBasket.Total);
    }

    [Fact]
    public void AddItem_ThrowsException_WhenProductAlreadyInBasketException()
    {
        SeedSingleTestBasketItem();
        var originalTotal = _testBasket.Total;

        Assert.Throws<ProductAlreadyInBasketException>(() =>
        {
            _testBasket.AddItem(_testItem);
        });
        Assert.Equal(originalTotal, _testBasket.Total);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1)]
    public void ChangeItemQuantity_UpdatesQuantity_WhenQuantityLargerThanZero(
        uint quantity)
    {
        SeedSingleTestBasketItem();

        _testBasket.ChangeItemQuantity(_testItem.ProductId, quantity);

        Assert.Equal(quantity, _testItem.Quantity);
        Assert.Contains(_testItem, _testBasket.Items);

        var changedTotal = quantity * _testItem.ProductUnitPrice;
        Assert.Equal(changedTotal, _testBasket.Total);
    }

    [Fact]
    public void ChangeItemQuantity_RemovesItem_WhenQuantityIsZero()
    {
        SeedSingleTestBasketItem();
        uint quantity = 0;

        _testBasket.ChangeItemQuantity(_testItem.ProductId, quantity);

        Assert.Equal(quantity, _testItem.Quantity);
        Assert.DoesNotContain(_testItem, _testBasket.Items);
        Assert.Equal(0, _testBasket.Total);
    }

    [Fact]
    public void ChangeItemQuantity_ThrowsException_WhenProductNotInBasket()
    {
        SeedSingleTestBasketItem();
        string productId = "PROD-1000";
        uint quantity = 20;

        Assert.Throws<ProductNotInBasketException>(() =>
        {
            _testBasket.ChangeItemQuantity(productId, quantity);
        });
        Assert.NotEqual(quantity, _testItem.Quantity);
    }

    [Fact]
    public void EmptyBasket_ClearsBasketAndUpdateTotal()
    {
        SeedSingleTestBasketItem();

        _testBasket.EmptyBasket();

        Assert.Empty(_testBasket.Items);
        Assert.Equal(0, _testBasket.Total);
    }

    // Helpers
    private void SeedSingleTestBasketItem()
    {
        _testBasket.AddItem(_testItem);
    }
}
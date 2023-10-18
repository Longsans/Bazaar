using Bazaar.Catalog.Domain.Exceptions;

namespace CatalogTests.UnitTests;

public class CatalogItemUnitTests
{
    #region Test data and helpers
    private const int _id = 1;
    private const string _productId = "PROD-1";
    private const string _productName = "Test";
    private const string _productDescription = "Test description";
    private const decimal _validPrice = 19.99m;
    private const string _partnerId = "PNER-1";

    private const uint _validRestockThreshold = 10;
    private const uint _validMaxStockThreshold = 500;

    private const uint _validStock = 100;
    private const uint _maxValidStock = _validMaxStockThreshold;

    private CatalogItem _testCatalogItem;

    private void InitializeTestData()
    {
        _testCatalogItem = new(
            _id, _productId, _productName, _productDescription,
            _validPrice, _validStock, _partnerId,
            _validRestockThreshold, _validMaxStockThreshold);
    }
    #endregion

    [Theory]
    [InlineData(_validStock)]
    [InlineData(_maxValidStock)]
    public void Constructor_Succeeds_WhenValid(uint stock)
    {
        _testCatalogItem = new CatalogItem(
            _id, _productId, _productName, _productDescription,
            _validPrice, stock, _partnerId,
            _validRestockThreshold, _validMaxStockThreshold);

        Assert.Equal(_id, _testCatalogItem.Id);
        Assert.Equal(_productId, _testCatalogItem.ProductId);
        Assert.Equal(_validPrice, _testCatalogItem.Price);
        Assert.Equal(stock, _testCatalogItem.AvailableStock);
        Assert.Equal(_validRestockThreshold, _testCatalogItem.RestockThreshold);
        Assert.Equal(_validMaxStockThreshold, _testCatalogItem.MaxStockThreshold);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-19.99)]
    public void Constructor_ThrowsArgumentException_WhenPriceNotPositive(decimal price)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _testCatalogItem = new CatalogItem(
                _id, _productId, _productName, _productDescription,
                price, _validStock, _partnerId,
                _validRestockThreshold, _validMaxStockThreshold);
        });
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenStockIsZero()
    {
        uint stock = 0;

        Assert.Throws<ArgumentException>(() =>
        {
            _testCatalogItem = new CatalogItem(
                _id, _productId, _productName, _productDescription,
                _validPrice, stock, _partnerId,
                _validRestockThreshold, _validMaxStockThreshold);
        });
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenMaxStockThresholdIsZero()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _testCatalogItem = new CatalogItem(
                _id, _productId, _productName, _productDescription,
                _validPrice, _validStock, _partnerId,
                _validRestockThreshold, 0);
        });
    }

    [Theory]
    [InlineData(_validMaxStockThreshold)]
    [InlineData(_validMaxStockThreshold + 1)]
    public void Constructor_ThrowsArgumentException_WhenRestockThresholdNotLessThanMaxStockThreshold(
        uint restockThreshold)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _testCatalogItem = new CatalogItem(
                _id, _productId, _productName, _productDescription,
                _validPrice, _validStock, _partnerId,
                restockThreshold, _validMaxStockThreshold);
        });
    }

    [Fact]
    public void Constructor_ThrowsExceedingMaxStockThresholdException_WhenStockGreaterThanMaxStockThreshold()
    {
        uint stock = _validMaxStockThreshold + 1;

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            _testCatalogItem = new CatalogItem(
                _id, _productId, _productName, _productDescription,
                _validPrice, stock, _partnerId,
                _validRestockThreshold, _validMaxStockThreshold);
        });
    }

    [Fact]
    public void ChangeProductDetails_Succeeds_WhenValid()
    {
        InitializeTestData();
        var newProductName = "Test 2";
        var newProductDescription = "Test description 2";
        var newPrice = 29.99m;

        _testCatalogItem.ChangeProductDetails(newProductName, newProductDescription, newPrice);

        Assert.Equal(newProductName, _testCatalogItem.ProductName);
        Assert.Equal(newProductDescription, _testCatalogItem.ProductDescription);
        Assert.Equal(newPrice, _testCatalogItem.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-29.99)]
    public void ChangeProductDetails_ThrowsArgumentException_WhenPriceIsZeroOrNegative(
        decimal newPrice)
    {
        InitializeTestData();
        var newProductName = "Test 2";
        var newProductDescription = "Test description 2";

        Assert.Throws<ArgumentException>(() =>
        {
            _testCatalogItem.ChangeProductDetails(newProductName, newProductDescription, newPrice);
        });

        Assert.NotEqual(newProductName, _testCatalogItem.ProductName);
        Assert.NotEqual(newProductDescription, _testCatalogItem.ProductDescription);
        Assert.NotEqual(newPrice, _testCatalogItem.Price);
    }

    [Theory]
    [InlineData(20)]
    [InlineData(_validStock)]
    public void ReduceStock_Succeeds_WhenValid(uint unitsToReduce)
    {
        InitializeTestData();
        uint remainingUnits = _testCatalogItem.AvailableStock - unitsToReduce;

        _testCatalogItem.ReduceStock(unitsToReduce);

        Assert.Equal(remainingUnits, _testCatalogItem.AvailableStock);
    }

    [Fact]
    public void ReduceStock_ThrowsArgumentException_WhenReduceUnitsIsZero()
    {
        InitializeTestData();
        uint unitsToReduce = 0;

        Assert.Throws<ArgumentException>(() =>
        {
            _testCatalogItem.ReduceStock(unitsToReduce);
        });
    }

    [Fact]
    public void ReduceStock_ThrowsNotEnoughStockException_WhenReduceUnitsGreaterThanStock()
    {
        InitializeTestData();
        uint unitsToReduce = 1000;

        Assert.Throws<NotEnoughStockException>(() =>
        {
            _testCatalogItem.ReduceStock(unitsToReduce);
        });
    }

    [Theory]
    [InlineData(100)]
    [InlineData(_maxValidStock - _validStock)]
    public void Restock_Succeeds_WhenValid(uint unitsToRestock)
    {
        InitializeTestData();
        uint unitsAfterRestock = _testCatalogItem.AvailableStock + unitsToRestock;

        _testCatalogItem.Restock(unitsToRestock);

        Assert.Equal(unitsAfterRestock, _testCatalogItem.AvailableStock);
    }

    [Fact]
    public void Restock_ThrowsArgumentException_WhenRestockUnitsIsZero()
    {
        InitializeTestData();
        uint unitsToRestock = 0;

        Assert.Throws<ArgumentException>(() =>
        {
            _testCatalogItem.Restock(unitsToRestock);
        });
    }

    [Fact]
    public void Restock_ThrowsExceedingMaxStockThresholdException_WhenRestockExceedsMaxStockThreshold()
    {
        InitializeTestData();
        uint unitsToRestock = _testCatalogItem.MaxStockThreshold - _testCatalogItem.AvailableStock + 1;

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            _testCatalogItem.Restock(unitsToRestock);
        });
    }

    [Theory]
    [InlineData(250)]
    [InlineData(_validStock)]
    public void ChangeStockThresholds_Succeeds_WhenValid(uint newMaxStockThreshold)
    {
        InitializeTestData();
        uint newRestockThreshold = 25;

        _testCatalogItem.ChangeStockThresholds(newRestockThreshold, newMaxStockThreshold);

        Assert.Equal(newRestockThreshold, _testCatalogItem.RestockThreshold);
        Assert.Equal(newMaxStockThreshold, _testCatalogItem.MaxStockThreshold);
    }

    [Fact]
    public void ChangeStockThresholds_ThrowsArgumentException_WhenMaxStockThresholdIsZero()
    {
        InitializeTestData();
        uint newRestockThreshold = 25;
        uint newMaxStockThreshold = 0;

        Assert.Throws<ArgumentException>(() =>
        {
            _testCatalogItem.ChangeStockThresholds(newRestockThreshold, newMaxStockThreshold);
        });
    }

    [Theory]
    [InlineData(120)]
    [InlineData(150)]
    public void ChangeStockThresholds_ThrowsArgumentException_WhenRestockThresholdNotLessThanMaxStockThreshold(
        uint newRestockThreshold)
    {
        InitializeTestData();

        uint newMaxStockThreshold = 120;

        Assert.Throws<ArgumentException>(() =>
        {
            _testCatalogItem.ChangeStockThresholds(newRestockThreshold, newMaxStockThreshold);
        });
    }

    [Fact]
    public void ChangeStockThresholds_ThrowsExceedingMaxStockThresholdException_WhenStockGreaterThanMaxStockThreshold()
    {
        InitializeTestData();
        uint newRestockThreshold = 25;
        uint newMaxStockThreshold = _testCatalogItem.AvailableStock - 1;

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            _testCatalogItem.ChangeStockThresholds(newRestockThreshold, newMaxStockThreshold);
        });
    }
}

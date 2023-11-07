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
    private const string _sellerId = "CLNT-1";

    private const uint _validStock = 100;
    private const bool _isNotFbb = false;

    private CatalogItem _testCatalogItem;

    private void InitializeTestData()
    {
        _testCatalogItem = new(
            _id, _productId, _productName, _productDescription,
            _validPrice, _validStock, _sellerId, false);
    }
    #endregion

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Constructor_Succeeds_WhenValid(bool isFbb)
    {
        _testCatalogItem = new CatalogItem(
            _id, _productId, _productName, _productDescription,
            _validPrice, _validStock, _sellerId, isFbb);

        Assert.Equal(_id, _testCatalogItem.Id);
        Assert.Equal(_productId, _testCatalogItem.ProductId);
        Assert.Equal(_validPrice, _testCatalogItem.Price);
        Assert.Equal(_validStock, _testCatalogItem.AvailableStock);
        Assert.Equal(isFbb, _testCatalogItem.IsFulfilledByBazaar);
        Assert.Equal(!_testCatalogItem.IsFulfilledByBazaar, _testCatalogItem.IsOfficiallyListed);
        Assert.False(_testCatalogItem.HasOrdersInProgress);
        Assert.False(_testCatalogItem.IsDeleted);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-19.99)]
    public void Constructor_ThrowsArgumentException_WhenPriceNotPositive(decimal price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _testCatalogItem = new CatalogItem(
                _id, _productId, _productName, _productDescription,
                price, _validStock, _sellerId, _isNotFbb);
        });
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenStockIsZero()
    {
        uint stock = 0;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _testCatalogItem = new CatalogItem(
                _id, _productId, _productName, _productDescription,
                _validPrice, stock, _sellerId, _isNotFbb);
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

    [Fact]
    public void ChangeProductDetails_ThrowsInvalidOpException_WhenItemIsDeleted()
    {
        InitializeTestData();
        _testCatalogItem.Delete();
        var newProductName = "Test 2";
        var newProductDescription = "Test description 2";
        var newPrice = 9.99m;

        Assert.Throws<InvalidOperationException>(() =>
        {
            _testCatalogItem.ChangeProductDetails(newProductName, newProductDescription, newPrice);
        });

        Assert.NotEqual(newProductName, _testCatalogItem.ProductName);
        Assert.NotEqual(newProductDescription, _testCatalogItem.ProductDescription);
        Assert.NotEqual(newPrice, _testCatalogItem.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-29.99)]
    public void ChangeProductDetails_ThrowsArgOutOfRangeException_WhenPriceIsZeroOrNegative(
        decimal newPrice)
    {
        InitializeTestData();
        var newProductName = "Test 2";
        var newProductDescription = "Test description 2";

        Assert.Throws<ArgumentOutOfRangeException>(() =>
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
    public void ReduceStock_ThrowsArgOutOfRangeException_WhenReduceUnitsIsZero()
    {
        InitializeTestData();
        uint unitsToReduce = 0;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _testCatalogItem.ReduceStock(unitsToReduce);
        });
    }

    [Fact]
    public void ReduceStock_ThrowsInvalidOpException_WhenItemIsDeleted()
    {
        InitializeTestData();
        _testCatalogItem.Delete();
        uint unitsToReduce = 2;

        Assert.Throws<InvalidOperationException>(() =>
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
    [InlineData(1000)]
    public void Restock_Succeeds_WhenValid(uint unitsToRestock)
    {
        InitializeTestData();
        uint unitsAfterRestock = _testCatalogItem.AvailableStock + unitsToRestock;

        _testCatalogItem.Restock(unitsToRestock);

        Assert.Equal(unitsAfterRestock, _testCatalogItem.AvailableStock);
    }

    [Fact]
    public void Restock_ThrowsInvalidOpException_WhenItemIsDeleted()
    {
        InitializeTestData();
        _testCatalogItem.Delete();
        uint unitsToRestock = 2;

        Assert.Throws<InvalidOperationException>(() =>
        {
            _testCatalogItem.Restock(unitsToRestock);
        });
    }

    [Fact]
    public void Restock_ThrowsArgOutOfRangeException_WhenRestockUnitsIsZero()
    {
        InitializeTestData();
        uint unitsToRestock = 0;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _testCatalogItem.Restock(unitsToRestock);
        });
    }
}

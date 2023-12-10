namespace CatalogTests.UnitTests;

public class CatalogItemUnitTests
{
    #region Test data and helpers
    private const string _productName = "Test";
    private const string _productDescription = "Test description";
    private const decimal _validPrice = 19.99m;
    private const string _sellerId = "CLNT-1";

    private const uint _validStock = 100;
    private const FulfillmentMethod _isMerchant = FulfillmentMethod.Merchant;

    private CatalogItem _testCatalogItem;

    private void InitializeTestData()
    {
        _testCatalogItem = new(
            _productName, _productDescription,
            _validPrice, _validStock, _sellerId, FulfillmentMethod.Merchant);
    }

    private void InitializeTestDataInStatus(ListingStatus status)
    {
        InitializeTestData();

        switch (status)
        {
            case ListingStatus.Active:
                return;
            case ListingStatus.InactiveOutOfStock:
                _testCatalogItem.ReduceStock(_testCatalogItem.AvailableStock);
                break;
            case ListingStatus.InactiveClosedListing:
                _testCatalogItem.CloseListing();
                break;
            case ListingStatus.Deleted:
                _testCatalogItem.Delete();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status));
        }
    }

    private void InitializeTestDataInStatusAndFulfillmentMethod(
        ListingStatus status, FulfillmentMethod method)
    {
        InitializeTestDataInStatus(status);
        typeof(CatalogItem).GetProperty(nameof(CatalogItem.FulfillmentMethod))!
            .SetValue(_testCatalogItem, method);
    }
    #endregion

    [Theory]
    [InlineData(FulfillmentMethod.Merchant)]
    [InlineData(FulfillmentMethod.Fbb)]
    public void Constructor_Succeeds_WhenValid(FulfillmentMethod ffMethod)
    {
        uint stock = ffMethod == FulfillmentMethod.Merchant
            ? _validStock : 0u;
        _testCatalogItem = new CatalogItem(
            _productName, _productDescription,
            _validPrice, stock, _sellerId, ffMethod);

        Assert.Equal(_validPrice, _testCatalogItem.Price);
        Assert.Equal(stock, _testCatalogItem.AvailableStock);
        Assert.Equal(ffMethod, _testCatalogItem.FulfillmentMethod);
        Assert.Equal(stock > 0u, _testCatalogItem.IsListingActive);
        Assert.Equal(stock == 0u, _testCatalogItem.IsOutOfStock);
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
                _productName, _productDescription,
                price, _validStock, _sellerId, _isMerchant);
        });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Constructor_ThrowsManualInsertOfFbbStockNotSupportedException_WhenIsFbbAndStockNotZero(uint stock)
    {
        Assert.Throws<ManualInsertOfFbbStockNotSupportedException>(() =>
        {
            _testCatalogItem = new CatalogItem(
                _productName, _productDescription,
                _validPrice, stock, _sellerId, FulfillmentMethod.Fbb);
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
    [InlineData(1)]
    [InlineData(20)]
    public void ReduceStock_Succeeds_WhenAllValidAndRemainingStockNotZero(uint unitsToReduce)
    {
        InitializeTestData();
        uint remainingUnits = _testCatalogItem.AvailableStock - unitsToReduce;

        _testCatalogItem.ReduceStock(unitsToReduce);

        Assert.Equal(remainingUnits, _testCatalogItem.AvailableStock);
        Assert.False(_testCatalogItem.IsOutOfStock);
    }

    [Fact]
    public void ReduceStock_SucceedsAndChangesStatusToOutOfStock_WhenAllValidAndRemainingStockIsZero()
    {
        InitializeTestData();

        _testCatalogItem.ReduceStock(_testCatalogItem.AvailableStock);

        Assert.Equal(0u, _testCatalogItem.AvailableStock);
        Assert.True(_testCatalogItem.IsOutOfStock);
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
    [InlineData(100, false)]
    [InlineData(1000, true)]
    public void Restock_Succeeds_WhenAllValid_And_ChangesStatusToActiveIfOriginalStatusWasOutOfStock(
        uint unitsToRestock, bool wasOutOfStock)
    {
        InitializeTestData();
        if (wasOutOfStock)
        {
            _testCatalogItem.ReduceStock(_testCatalogItem.AvailableStock);
        }
        uint unitsAfterRestock = _testCatalogItem.AvailableStock + unitsToRestock;


        _testCatalogItem.Restock(unitsToRestock);

        Assert.Equal(unitsAfterRestock, _testCatalogItem.AvailableStock);
        Assert.True(_testCatalogItem.IsListingActive);
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

    [Theory]
    [InlineData(ListingStatus.Active)]
    [InlineData(ListingStatus.InactiveOutOfStock)]
    [InlineData(ListingStatus.InactiveClosedListing)]
    public void CloseListing_Succeeds_WhenListingNotDeleted(ListingStatus status)
    {
        InitializeTestDataInStatus(status);

        _testCatalogItem.CloseListing();

        Assert.True(_testCatalogItem.IsListingClosed);
    }

    [Fact]
    public void CloseListing_ThrowsInvalidOpException_WhenListingIsDeleted()
    {
        InitializeTestDataInStatus(ListingStatus.Deleted);

        Assert.Throws<InvalidOperationException>(_testCatalogItem.CloseListing);
    }

    [Fact]
    public void Relist_Succeeds_WhenListingClosed()
    {
        InitializeTestData();
        _testCatalogItem.CloseListing();

        _testCatalogItem.Relist();

        Assert.True(_testCatalogItem.IsListingActive);
    }

    [Theory]
    [InlineData(ListingStatus.Active)]
    [InlineData(ListingStatus.InactiveOutOfStock)]
    [InlineData(ListingStatus.Deleted)]
    public void Relist_ThrowsInvalidOpException_WhenListingNotClosed(ListingStatus status)
    {
        InitializeTestDataInStatus(status);

        Assert.Throws<InvalidOperationException>(_testCatalogItem.Relist);
    }


    [Theory]
    [InlineData(ListingStatus.Active)]
    [InlineData(ListingStatus.InactiveOutOfStock)]
    public void ChangeFulfillmentMethodToFbb_SucceedsAndReducesStockToZero_WhenAllValid(ListingStatus status)
    {
        InitializeTestDataInStatus(status);

        _testCatalogItem.ChangeFulfillmentMethodToFbb();

        Assert.True(_testCatalogItem.IsFbb);
        Assert.True(_testCatalogItem.IsOutOfStock);
    }

    [Theory]
    [InlineData(ListingStatus.Deleted, FulfillmentMethod.Merchant)]
    [InlineData(ListingStatus.InactiveClosedListing, FulfillmentMethod.Merchant)]
    [InlineData(ListingStatus.Active, FulfillmentMethod.Fbb)]
    public void ChangeFulfillmentMethodToFbb_ThrowsInvalidOpException_WhenListingDeletedOrClosedOrFulfillmentMethodIsAlreadyFbb(
        ListingStatus status, FulfillmentMethod ffMethod)
    {
        InitializeTestDataInStatusAndFulfillmentMethod(status, ffMethod);

        Assert.Throws<InvalidOperationException>(_testCatalogItem.ChangeFulfillmentMethodToFbb);
    }

    [Theory]
    [InlineData(ListingStatus.Active)]
    [InlineData(ListingStatus.InactiveOutOfStock)]
    public void ChangeFulfillmentMethodToMerchant_Succeeds_WhenAllValid(ListingStatus status)
    {
        InitializeTestDataInStatusAndFulfillmentMethod(status, FulfillmentMethod.Fbb);

        _testCatalogItem.ChangeFulfillmentMethodToMerchant();

        Assert.Equal(FulfillmentMethod.Merchant, _testCatalogItem.FulfillmentMethod);
    }

    [Theory]
    [InlineData(ListingStatus.Deleted)]
    [InlineData(ListingStatus.InactiveClosedListing)]
    public void ChangeFulfillmentMethodToMerchant_ThrowsInvalidOpException_WhenListingDeletedOrClosed(
        ListingStatus status)
    {
        InitializeTestDataInStatus(status);

        Assert.Throws<InvalidOperationException>(_testCatalogItem.ChangeFulfillmentMethodToMerchant);
    }
}

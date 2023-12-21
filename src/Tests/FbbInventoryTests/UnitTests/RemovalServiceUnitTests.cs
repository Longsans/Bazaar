using Bazaar.FbbInventory.Application.DTOs;

namespace FbbInventoryTests.UnitTests;

public class RemovalServiceUnitTests
{
    private readonly RemovalService _service;
    private readonly Mock<IProductInventoryRepository> _prodInventoryRepoMock;
    private readonly Mock<ILotRepository> _lotRepoMock;
    private readonly EventBusTestDouble _testEventBus;

    #region Test data and helpers
    private const string _validAddress = "308 Negra Arroyo Lane, Albuquerque, New Mexico, U.S.A";
    private readonly ProductInventory _testProdInventory = new("PROD-1", 100, 5, 10, 10, 1000, 1);
    private readonly SellerInventory _testSellerInventory = new("CLNT-1");
    private readonly List<Lot> _testLots;

    private static void SetupPastLots(ProductInventory prodInventory,
        uint fulfillableUnits, uint unfulfillableUnits)
    {
        var pastFfLot = new Lot(prodInventory, fulfillableUnits / 2);
        var pastFfLot2 = new Lot(prodInventory, fulfillableUnits / 2);
        var pastUfLot = new Lot(prodInventory,
            UnfulfillableCategory.Defective, unfulfillableUnits / 2);
        var pastUfLot2 = new Lot(prodInventory,
            UnfulfillableCategory.Defective, unfulfillableUnits / 2);
        typeof(Lot).GetProperty(nameof(Lot.DateUnitsEnteredStorage))!
            .SetValue(pastFfLot, DateTime.Now.AddDays(-3));
        typeof(Lot).GetProperty(nameof(Lot.DateUnitsBecameUnfulfillable))!
            .SetValue(pastUfLot, DateTime.Now.AddDays(-3));
        typeof(Lot).GetProperty(nameof(Lot.DateUnitsEnteredStorage))!
            .SetValue(pastFfLot2, DateTime.Now.AddDays(-2));
        typeof(Lot).GetProperty(nameof(Lot.DateUnitsBecameUnfulfillable))!
            .SetValue(pastUfLot2, DateTime.Now.AddDays(-2));

        var ffLots = typeof(ProductInventory)
            .GetField("_lots", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(prodInventory) as List<Lot>;
        var ufLots = typeof(ProductInventory)
            .GetField("_lots", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(prodInventory) as List<Lot>;
        ffLots.Add(pastFfLot);
        ufLots.Add(pastUfLot);
        ffLots.Add(pastFfLot2);
        ufLots.Add(pastUfLot2);
    }
    #endregion

    public RemovalServiceUnitTests(EventBusTestDouble testEventBus)
    {
        var prodInventories = typeof(SellerInventory)
            .GetProperty(nameof(SellerInventory.ProductInventories))!
            .GetValue(_testSellerInventory) as List<ProductInventory>;
        prodInventories.Add(_testProdInventory);
        typeof(ProductInventory).GetProperty(nameof(ProductInventory.SellerInventory))!
            .SetValue(_testProdInventory, _testSellerInventory);

        _testLots = new()
        {
            new Lot(_testProdInventory, 50),
            new Lot(_testProdInventory, 100),
            new Lot(_testProdInventory, 200),
        };
        for (int i = 0; i < 3; i++)
        {
            typeof(Lot).GetProperty(nameof(Lot.LotNumber))!
                .SetValue(_testLots[i], $"FUFL-{i + 1}");
        }

        _testEventBus = testEventBus;
        _lotRepoMock = new();
        _prodInventoryRepoMock = new();

        var sellerInventoryRepoMock = new Mock<ISellerInventoryRepository>();
        _service = new(sellerInventoryRepoMock.Object,
            _prodInventoryRepoMock.Object, _lotRepoMock.Object, _testEventBus);
    }

    [Theory]
    [InlineData(25, 10)]    // no lots removed
    [InlineData(75, 25)]    // 1 lot from each storage + extra 25 FF & 10 UF
    [InlineData(125, 30)]   // 2 lots from each storage + extra 25 FF & 10 UF
    [InlineData(200, 45)]   // all lots removed
    public void RequestRemovalForProductStocksFromOldToNew_LabelsLotUnitsFromOldToNewAndReturnsSuccess_WhenAllValid(
        uint fulfillableUnits, uint unfulfillableUnits)
    {
        SetupPastLots(_testProdInventory, 100, 30); // total 3 FF lots (50, 50, 100) and 4 UF lots (15, 15, 10, 5)
        _prodInventoryRepoMock.Setup(x => x.GetByProductId(It.IsAny<string>()))
            .Returns(_testProdInventory);
        var removalDtos = new List<StockUnitsRemovalDto>
        {
            new("PROD-1", fulfillableUnits, unfulfillableUnits),
        };

        var result = _service.SendProductStocksForReturn(removalDtos, _validAddress);

        var removalFfLots = _testProdInventory.FulfillableLots
            .Where(x => !x.HasUnitsInStock);
        var removalUfLots = _testProdInventory.UnfulfillableLots
            .Where(x => !x.HasUnitsInStock);
        var remainingFfLots = _testProdInventory.Lots.Except(removalFfLots);
        var remainingUfLots = _testProdInventory.Lots.Except(removalUfLots);
        Assert.DoesNotContain(removalFfLots, fLot =>
            remainingFfLots.Any(x => x.DateUnitsEnteredStorage < fLot.DateUnitsEnteredStorage));
        Assert.DoesNotContain(removalUfLots, uLot =>
            remainingUfLots.Any(x => x.DateUnitsBecameUnfulfillable < uLot.DateUnitsBecameUnfulfillable));

        Assert.Equal(fulfillableUnits, _testProdInventory.FulfillablePendingRemovalUnits);
        Assert.Equal(unfulfillableUnits, _testProdInventory.UnfulfillablePendingRemovalUnits);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RequestReturnForProductStocksFromOldToNew_ReturnsInvalid_WhenDeliveryAddressEmpty(
        string deliveryAddress)
    {
        var result = _service.SendProductStocksForReturn(
            new List<StockUnitsRemovalDto>
            {
                new("PROD-1", 10, 10)
            }, deliveryAddress);

        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public void RequestReturnForProductStocksFromOldToNew_ReturnsInvalid_WhenDuplicateProducts()
    {
        var removalDtos = new List<StockUnitsRemovalDto>
        {
            new("PROD-1", 1, 1),
            new("PROD-1", 1, 1),
            new("PROD-2", 1, 1),
        };

        var result = _service.SendProductStocksForReturn(removalDtos, _validAddress);

        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public void RequestRemovalForProductStocksFromOldToNew_ReturnsConflict_WhenFulfillableStockNotEnough()
    {
        _prodInventoryRepoMock.Setup(x => x.GetByProductId(It.IsAny<string>()))
            .Returns(_testProdInventory);
        var removalDtos = new List<StockUnitsRemovalDto>
        {
            new("PROD-1", _testProdInventory.FulfillableUnits + 1, 0)
        };

        var result = _service.SendProductStocksForReturn(removalDtos, _validAddress);

        Assert.Equal(ResultStatus.Conflict, result.Status);
    }

    [Fact]
    public void RequestRemovalForProductStocksFromOldToNew_ReturnsConflict_WhenUnfulfillableStockNotEnough()
    {
        _prodInventoryRepoMock.Setup(x => x.GetByProductId(It.IsAny<string>()))
            .Returns(_testProdInventory);
        var removalDtos = new List<StockUnitsRemovalDto>
        {
            new("PROD-1", 0, _testProdInventory.UnfulfillableUnits + 1)
        };

        var result = _service.SendProductStocksForReturn(removalDtos, _validAddress);

        Assert.Equal(ResultStatus.Conflict, result.Status);
    }

    [Fact]
    public void RequestReturnForLots_LabelsLotsForRemovalAndReturnsSuccess_WhenAllValid()
    {
        _lotRepoMock.Setup(x => x.GetManyByLotNumber(It.IsAny<IEnumerable<string>>()))
            .Returns(_testLots);
        var lotsBeforeLabeling = _testLots.Select(x => new
        {
            Lot = x,
            OriginalUnitsInStock = x.UnitsInStock
        }).ToList();

        var result = _service.RequestReturnForLots(new string[] { "FUFL-1" }, _validAddress);

        Assert.True(result.IsSuccess);
        Assert.True(lotsBeforeLabeling.All(x => !x.Lot.HasUnitsInStock
            && x.Lot.PendingRemovalUnits == x.OriginalUnitsInStock));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RequestReturnForLots_ReturnsInvalid_WhenDeliveryAddressEmpty(
        string deliveryAddress)
    {
        var result = _service.RequestReturnForLots(
            new string[] { "FUFL-1" }, deliveryAddress);

        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public void RequestReturnForLots_ReturnsInvalid_WhenDuplicateLotNumbers()
    {
        var lotNumbers = new string[]
        {
            "FUFL-1",
            "FUFL-1",
            "FUFL-2",
        };

        var result = _service.RequestReturnForLots(lotNumbers, _validAddress);

        Assert.Equal(ResultStatus.Invalid, result.Status);
    }

    [Fact]
    public void RequestReturnForLots_ReturnsConflict_WhenALotHasNoUnitsInStockToLabel()
    {
        var lotWithNoUnits = _testLots.First();
        typeof(Lot).GetProperty(nameof(Lot.UnitsInStock))!
            .SetValue(lotWithNoUnits, 0u);
        _lotRepoMock.Setup(x => x.GetManyByLotNumber(It.IsAny<IEnumerable<string>>()))
            .Returns(_testLots);

        var lotNumbers = new string[]
        {
            "FUFL-1",
            "FUFL-2",
            "FUFL-3",
        };

        var result = _service.RequestReturnForLots(lotNumbers, _validAddress);

        Assert.Equal(ResultStatus.Conflict, result.Status);
    }
}

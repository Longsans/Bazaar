namespace TransportTests.UnitTests;

public class BasicEstimationServiceUnitTests
{
    #region Test data and helpers
    private static readonly IEnumerable<Delivery> _validDeliveries = new Delivery[]
    {
        new(1, "Test address", new DeliveryPackageItem[]
        {
            new("PROD-1", 1),
            new("PROD-2", 2),
        }, DateTime.Now.AddDays(5)),
        new(2, "Test address 2", new DeliveryPackageItem[]
        {
            new("PROD-3", 3),
            new("PROD-4", 4),
        }, DateTime.Now.AddDays(6)),
    };
    private static readonly IEnumerable<InventoryPickup> _validPickups = new InventoryPickup[]
    {
        new("Test location", new ProductInventory[]
        {
            new("PROD-1", 100),
            new("PROD-2", 200),
        }, DateTime.Now.AddDays(7), "CLNT-1"),
        new("Test location 2", new ProductInventory[]
        {
            new("PROD-3", 300),
            new("PROD-4", 400),
        }, DateTime.Now.AddDays(7), "CLNT-1"),
    };
    private static readonly TimeSpan _delayPerDeliveryItem = TimeSpan.FromMinutes(4);
    private static readonly TimeSpan _delayPerPickupItem = TimeSpan.FromMinutes(1.2);

    private static bool SameTime(DateTime a, DateTime b) => a - b < TimeSpan.FromSeconds(1);
    #endregion

    private readonly Mock<IDeliveryRepository> _mockDeliveryRepo;
    private readonly Mock<IInventoryPickupRepository> _mockPickupRepo;
    private readonly BasicEstimationService _service;

    public BasicEstimationServiceUnitTests()
    {
        _mockDeliveryRepo = new();
        _mockPickupRepo = new();
        _service = new(_mockDeliveryRepo.Object, _mockPickupRepo.Object);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EstimateDeliveryCompletion_Returns4MinutesForEveryItemToBeDelivered(
        bool hasExistingDeliveries)
    {
        // arrange
        var currentDeliveries = hasExistingDeliveries
            ? _validDeliveries
            : Array.Empty<Delivery>();

        _mockDeliveryRepo.Setup(x => x.GetIncomplete())
            .Returns(currentDeliveries);

        var packageItems = new DeliveryPackageItem[]
        {
            new("PROD-1", 1),
            new("PROD-2", 2),
        };

        // act
        var estimatedTime = _service.EstimateDeliveryCompletion(packageItems);

        // assert
        var expectedTime = DateTime.Now + (hasExistingDeliveries ? 13 : 3) * _delayPerDeliveryItem;

        Assert.True(SameTime(expectedTime, estimatedTime));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EstimatePickupCompletion_Returns1Minute12SecondsForEveryItemToBeDelivered(
        bool hasExistingPickups)
    {
        // arrange
        var currentPickups = hasExistingPickups
            ? _validPickups
            : Array.Empty<InventoryPickup>();

        _mockPickupRepo.Setup(x => x.GetIncomplete())
            .Returns(currentPickups);

        var productInventories = new ProductInventory[]
        {
            new("PROD-1", 50),
            new("PROD-2", 100),
        };

        // act
        var estimatedTime = _service.EstimatePickupCompletion(productInventories);

        // assert
        var expectedTime = DateTime.Now + (hasExistingPickups ? 1150 : 150) * _delayPerPickupItem;

        Assert.True(SameTime(expectedTime, estimatedTime));
    }
}

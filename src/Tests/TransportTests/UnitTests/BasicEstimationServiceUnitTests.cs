﻿using Bazaar.Transport.Domain.Specifications;
using Microsoft.Extensions.Configuration;

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
        new("Test location", new PickupProductStock[]
        {
            new("PROD-1", 100),
            new("PROD-2", 200),
        }, DateTime.Now.AddDays(7), "CLNT-1"),
        new("Test location 2", new PickupProductStock[]
        {
            new("PROD-3", 300),
            new("PROD-4", 400),
        }, DateTime.Now.AddDays(7), "CLNT-1"),
    };
    private static readonly IEnumerable<InventoryReturn> _validReturns = new InventoryReturn[]
    {
        new("Test location", new ReturnQuantity[]
        {
            new("LOT-1", 100),
            new("Lot-2", 200),
        }, DateTime.Now.AddDays(7), "CLNT-1"),
        new("Test location 2", new ReturnQuantity[]
        {
            new("LOT-3", 300),
            new("LOT-4", 400),
        }, DateTime.Now.AddDays(7), "CLNT-1"),
    };
    private static readonly TimeSpan _delayPerDeliveryItem = TimeSpan.FromMinutes(4);
    private static readonly TimeSpan _delayPerPickupItem = TimeSpan.FromMinutes(1.2);
    private static readonly TimeSpan _delayPerReturnItem = TimeSpan.FromMinutes(2);

    private static bool SameTime(DateTime a, DateTime b) => a - b < TimeSpan.FromSeconds(1);
    #endregion

    private readonly Mock<IRepository<Delivery>> _mockDeliveryRepo;
    private readonly Mock<IRepository<InventoryPickup>> _mockPickupRepo;
    private readonly Mock<IRepository<InventoryReturn>> _mockReturnRepo;
    private readonly BasicEstimationService _service;

    public BasicEstimationServiceUnitTests()
    {
        _mockDeliveryRepo = new();
        _mockPickupRepo = new();
        _mockReturnRepo = new();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TimePerDeliveryItem", _delayPerDeliveryItem.TotalMinutes.ToString() },
                { "TimePerPickupItem", _delayPerPickupItem.TotalMinutes.ToString() },
                { "TimePerReturnUnit", _delayPerReturnItem.TotalMinutes.ToString() }
            })
            .Build();
        _service = new(_mockDeliveryRepo.Object, _mockPickupRepo.Object,
            _mockReturnRepo.Object, config);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EstimateDeliveryCompletion_Returns4MinutesForEveryItemToBeDelivered(
        bool hasExistingDeliveries)
    {
        // arrange
        var currentDeliveries = hasExistingDeliveries
            ? _validDeliveries
            : Array.Empty<Delivery>();

        _mockDeliveryRepo.Setup(x => x.ListAsync(
                It.IsAny<DeliveriesIncompleteSpec>(), CancellationToken.None))
            .Returns(Task.FromResult(currentDeliveries.ToList()));

        var packageItems = new DeliveryPackageItem[]
        {
            new("PROD-5", 5),
            new("PROD-6", 6),
        };

        // act
        var estimatedTime = await _service.EstimateDeliveryCompletion(packageItems);

        // assert
        var expectedTime = DateTime.Now + (hasExistingDeliveries ? 13 : 3) * _delayPerDeliveryItem;
        Assert.True(SameTime(expectedTime, estimatedTime));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EstimatePickupCompletion_Returns1Minute12SecondsForEveryItemToBeDelivered(
        bool hasExistingPickups)
    {
        // arrange
        var currentPickups = hasExistingPickups
            ? _validPickups
            : Array.Empty<InventoryPickup>();

        _mockPickupRepo.Setup(x => x.ListAsync(
                It.IsAny<PickupsIncompleteSpec>(), CancellationToken.None))
            .Returns(Task.FromResult(currentPickups.ToList()));

        var productInventories = new PickupProductStock[]
        {
            new("PROD-5", 50),
            new("PROD-6", 60),
        };

        // act
        var estimatedTime = await _service.EstimatePickupCompletion(productInventories);

        // assert
        var expectedTime = DateTime.Now + (hasExistingPickups ? 1110 : 110) * _delayPerPickupItem;
        Assert.True(SameTime(expectedTime, estimatedTime));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EstimateInventoryReturnCompletion_Returns2MinutesForEveryItemToBeReturned(
        bool hasExistingReturns)
    {
        // arrange
        var currentReturns = hasExistingReturns
            ? _validReturns
            : Array.Empty<InventoryReturn>();
        _mockReturnRepo.Setup(x => x.ListAsync(
                It.IsAny<ReturnsIncompleteSpec>(), CancellationToken.None))
            .Returns(Task.FromResult(currentReturns.ToList()));

        var returnQuantities = new ReturnQuantity[]
        {
            new("LOT-5", 55),
            new("LOT-6", 66),
        };

        // act
        var estimatedTime = await _service.EstimateInventoryReturnCompletion(returnQuantities);

        // assert
        var expectedTime = DateTime.Now + (hasExistingReturns ? 1121 : 121) * _delayPerReturnItem;
        Assert.True(SameTime(expectedTime, estimatedTime));
    }
}

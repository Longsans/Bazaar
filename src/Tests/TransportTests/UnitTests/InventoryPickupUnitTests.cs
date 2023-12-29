namespace TransportTests.UnitTests;

public class InventoryPickupUnitTests
{
    #region Test data and helpers
    private const string _validLocation = "11 Wall St, New York, NY 10005, USA";
    private static readonly List<PickupProductStock> _validInventories = new()
    {
        new("PROD-1", 100),
        new("PROD-2", 200),
        new("PROD-3", 300),
    };
    private static readonly DateTime _validPickupTime = DateTime.Now.AddDays(10);
    private const string _validSchedulerId = "CLNT-1";
    private const string _validCancelReason = "Transport system overloaded.";

    private InventoryPickup GetTestPickup(InventoryPickupStatus? statusToSet = null)
    {
        var pickup = new InventoryPickup(_validLocation, _validInventories,
            _validPickupTime, _validSchedulerId);

        if (statusToSet != null)
        {
            typeof(InventoryPickup).GetProperty(nameof(pickup.Status))!
                .SetValue(pickup, statusToSet);
        }
        return pickup;
    }
    #endregion

    [Fact]
    public void Constructor_SucceedsWithScheduledAtNowAndScheduledStatusAndNullCancelReason_WhenAllValid()
    {
        var pickup = new InventoryPickup(_validLocation, _validInventories,
            _validPickupTime, _validSchedulerId);

        Assert.True(DateTime.Now - pickup.TimeScheduledAt < TimeSpan.FromSeconds(1));
        Assert.Equal(InventoryPickupStatus.Scheduled, pickup.Status);
        Assert.Null(pickup.CancelReason);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ThrowsArgNullException_WhenPickupLocationIsNullOrEmpty(
        string pickupLocation)
    {
        Assert.Throws<ArgumentNullException>(nameof(pickupLocation), () =>
        {
            var pickup = new InventoryPickup(pickupLocation, _validInventories,
            _validPickupTime, _validSchedulerId);
        });
    }

    [Fact]
    public void Constructor_ThrowsArgOutOfRangeException_WhenEstimatedPickupTimeBeforeCurrentTime()
    {
        var estimatedPickupTime = DateTime.Now.AddMinutes(-1);

        Assert.Throws<ArgumentOutOfRangeException>(nameof(estimatedPickupTime), () =>
        {
            var pickup = new InventoryPickup(_validLocation, _validInventories,
            estimatedPickupTime, _validSchedulerId);
        });
    }

    [Fact]
    public void Constructor_ThrowsArgNullException_WhenInventoryListEmpty()
    {
        var productInventories = new List<PickupProductStock>();

        Assert.Throws<ArgumentNullException>(nameof(productInventories), () =>
        {
            var pickup = new InventoryPickup(_validLocation, productInventories,
            _validPickupTime, _validSchedulerId);
        });
    }

    [Theory]
    [InlineData("PROD-1")]
    [InlineData("PROD-2")]
    [InlineData("PROD-3")]
    public void Constructor_ThrowsArgException_WhenInventoriesHaveDuplicateProducts(
        string duplicateProductId)
    {
        var productInventories = new List<PickupProductStock>()
        {
            new(duplicateProductId, 100),
            new(duplicateProductId, 200),
        };

        Assert.Throws<ArgumentException>(nameof(productInventories), () =>
        {
            var pickup = new InventoryPickup(_validLocation, productInventories,
            _validPickupTime, _validSchedulerId);
        });
    }

    [Fact]
    public void StartPickup_ChangesStatusToEnRouteToPickupLocation_WhenInitialStatusIsScheduled()
    {
        var pickup = GetTestPickup();

        pickup.StartPickup();

        Assert.Equal(InventoryPickupStatus.EnRouteToPickupLocation, pickup.Status);
    }

    [Theory]
    [InlineData(InventoryPickupStatus.EnRouteToPickupLocation)]
    [InlineData(InventoryPickupStatus.DeliveringToWarehouse)]
    [InlineData(InventoryPickupStatus.Completed)]
    [InlineData(InventoryPickupStatus.Cancelled)]
    public void StartPickup_ThrowsInvalidOpException_WhenInitialStatusIsNotScheduled(
        InventoryPickupStatus initialStatus)
    {
        var pickup = GetTestPickup(initialStatus);

        Assert.Throws<InvalidOperationException>(pickup.StartPickup);
    }

    [Fact]
    public void ConfirmInventoryPickedUp_ChangesStatusToDeliveringToWarehouse_WhenInitialStatusIsEnRoute()
    {
        var pickup = GetTestPickup(InventoryPickupStatus.EnRouteToPickupLocation);

        pickup.ConfirmInventoryPickedUp();

        Assert.Equal(InventoryPickupStatus.DeliveringToWarehouse, pickup.Status);
    }

    [Theory]
    [InlineData(InventoryPickupStatus.Scheduled)]
    [InlineData(InventoryPickupStatus.DeliveringToWarehouse)]
    [InlineData(InventoryPickupStatus.Completed)]
    [InlineData(InventoryPickupStatus.Cancelled)]
    public void ConfirmInventoryPickedUp_ThrowsInvalidOpException_WhenInitialStatusIsNotEnRoute(
        InventoryPickupStatus initialStatus)
    {
        var pickup = GetTestPickup(initialStatus);

        Assert.Throws<InvalidOperationException>(pickup.ConfirmInventoryPickedUp);
    }

    [Fact]
    public void Complete_ChangesStatusToCompleted_WhenInitialStatusIsDelivering()
    {
        var pickup = GetTestPickup(InventoryPickupStatus.DeliveringToWarehouse);

        pickup.Complete();

        Assert.Equal(InventoryPickupStatus.Completed, pickup.Status);
    }

    [Theory]
    [InlineData(InventoryPickupStatus.Scheduled)]
    [InlineData(InventoryPickupStatus.EnRouteToPickupLocation)]
    [InlineData(InventoryPickupStatus.Completed)]
    [InlineData(InventoryPickupStatus.Cancelled)]
    public void Complete_ThrowsInvalidOpException_WhenInitialStatusIsNotDelivering(
        InventoryPickupStatus initialStatus)
    {
        var pickup = GetTestPickup(initialStatus);

        Assert.Throws<InvalidOperationException>(pickup.Complete);
    }

    [Theory]
    [InlineData(InventoryPickupStatus.Scheduled)]
    [InlineData(InventoryPickupStatus.EnRouteToPickupLocation)]
    [InlineData(InventoryPickupStatus.DeliveringToWarehouse)]
    public void Cancel_ChangesStatusToCancelled_WhenInitialStatusIsNotCompletedNorCancelledAndCancelReasonNotEmpty(
        InventoryPickupStatus initialStatus)
    {
        var pickup = GetTestPickup(initialStatus);

        pickup.Cancel(_validCancelReason);

        Assert.Equal(InventoryPickupStatus.Cancelled, pickup.Status);
        Assert.Equal(_validCancelReason, pickup.CancelReason);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cancel_ThrowsArgNullException_WhenCancelReasonIsNullOrEmpty(string cancelReason)
    {
        var pickup = GetTestPickup();

        Assert.Throws<ArgumentNullException>(() =>
        {
            pickup.Cancel(cancelReason);
        });
    }

    [Theory]
    [InlineData(InventoryPickupStatus.Completed)]
    [InlineData(InventoryPickupStatus.Cancelled)]
    public void Cancel_ThrowsInvalidOpException_WhenInitialStatusIsCompletedOrCancelled(
        InventoryPickupStatus initialStatus)
    {
        var pickup = GetTestPickup(initialStatus);

        Assert.Throws<InvalidOperationException>(() => pickup.Cancel(_validCancelReason));
    }
}

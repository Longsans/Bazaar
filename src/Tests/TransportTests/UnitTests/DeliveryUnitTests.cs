namespace TransportTests.UnitTests;

public class DeliveryUnitTests
{
    #region Test data and helpers
    private const int _validOrderId = 1;
    private const string _validAddress = "308 Negra Arroyo Lane, Albuquerque, New Mexico, U.S.";
    private static readonly List<DeliveryPackageItem> _validPackageItems = new()
    {
        new ("PROD-1", 2),
        new ("PROD-2", 1),
    };
    private static readonly DateTime _validDeliveryDate = DateTime.Now.AddDays(5).Date;

    private static Delivery GetTestDelivery(DeliveryStatus statusToSet)
    {
        var delivery = new Delivery(_validOrderId, _validAddress,
            _validPackageItems, _validDeliveryDate);
        typeof(Delivery).GetProperty(nameof(delivery.Status))!
            .SetValue(delivery, statusToSet);
        return delivery;
    }
    #endregion

    [Fact]
    public void Constructor_SucceedsWithScheduledDateIsNowAndScheduledStatus_WhenAllValid()
    {
        var delivery = new Delivery(
            _validOrderId, _validAddress, _validPackageItems, _validDeliveryDate);

        ExtendedAssert.SameTime(DateTime.Now, delivery.TimeScheduledAt);
        Assert.Equal(_validDeliveryDate, delivery.EstimatedDeliveryTime);
        Assert.Equal(DeliveryStatus.Scheduled, delivery.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_ThrowsArgException_WhenDeliveryAddressEmptyOrWhitespaces(
        string deliveryAddress)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var delivery = new Delivery(
                _validOrderId, deliveryAddress, _validPackageItems, _validDeliveryDate);
        });
    }

    [Fact]
    public void Constructor_ThrowsArgException_WhenPackageItemsEmpty()
    {
        var packageItems = new List<DeliveryPackageItem>();

        Assert.Throws<ArgumentException>(() =>
        {
            var delivery = new Delivery(
                _validOrderId, _validAddress, packageItems, _validDeliveryDate);
        });
    }

    [Fact]
    public void Constructor_ThrowsArgOutOfRangeException_WhenExpectedDeliveryDateBeforeCurrentDate()
    {
        var expectedDeliveryDate = DateTime.Now.AddDays(-1).Date;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var delivery = new Delivery(
                _validOrderId, _validAddress, _validPackageItems, expectedDeliveryDate);
        });
    }

    [Theory]
    [InlineData("PROD-1")]
    [InlineData("PROD-2")]
    [InlineData("PROD-3")]
    public void Constructor_ThrowsArgumentException_WhenPackageItemsHaveDuplicateProducts(
        string duplicateProductId)
    {
        var packageItems = new List<DeliveryPackageItem>
        {
            new(duplicateProductId, 1),
            new(duplicateProductId, 2)
        };

        Assert.Throws<ArgumentException>(() =>
        {
            var delivery = new Delivery(
                _validOrderId, _validAddress, packageItems, _validDeliveryDate);
        });
    }

    [Theory]
    [InlineData(DeliveryStatus.Scheduled)]
    [InlineData(DeliveryStatus.Postponed)]
    public void StartDelivery_ChangesStatusToDelivering_WhenInitialStatusIsScheduledOrPostponed(
        DeliveryStatus initialStatus)
    {
        var delivery = GetTestDelivery(initialStatus);

        delivery.StartDelivery();

        Assert.Equal(DeliveryStatus.Delivering, delivery.Status);
    }

    [Theory]
    [InlineData(DeliveryStatus.Delivering)]
    [InlineData(DeliveryStatus.Completed)]
    [InlineData(DeliveryStatus.Cancelled)]
    public void StartDelivery_ThrowsInvalidOpException_WhenInitialStatusIsNotScheduledNorPostponed(
        DeliveryStatus initialStatus)
    {
        var delivery = GetTestDelivery(initialStatus);

        Assert.Throws<InvalidOperationException>(delivery.StartDelivery);
    }

    [Fact]
    public void Complete_ChangesStatusToCompleted_WhenInitialStatusIsDelivering()
    {
        var delivery = GetTestDelivery(DeliveryStatus.Delivering);

        delivery.Complete();

        Assert.Equal(DeliveryStatus.Completed, delivery.Status);
    }

    [Theory]
    [InlineData(DeliveryStatus.Scheduled)]
    [InlineData(DeliveryStatus.Completed)]
    [InlineData(DeliveryStatus.Postponed)]
    [InlineData(DeliveryStatus.Cancelled)]
    public void Complete_ThrowsInvalidOpException_WhenInitialStatusIsNotDelivering(
        DeliveryStatus initialStatus)
    {
        var delivery = GetTestDelivery(initialStatus);

        Assert.Throws<InvalidOperationException>(delivery.Complete);
    }

    [Theory]
    [InlineData(DeliveryStatus.Scheduled)]
    [InlineData(DeliveryStatus.Delivering)]
    [InlineData(DeliveryStatus.Postponed)]
    public void Postpone_ChangesStatusToPostponed_WhenInitialStatusIsNotCompletedNorCancelled(
        DeliveryStatus initialStatus)
    {
        var delivery = GetTestDelivery(initialStatus);

        delivery.Postpone();

        Assert.Equal(DeliveryStatus.Postponed, delivery.Status);
    }

    [Theory]
    [InlineData(DeliveryStatus.Completed)]
    [InlineData(DeliveryStatus.Cancelled)]
    public void Postpone_ThrowsInvalidOpException_WhenInitialStatusIsCompletedOrCancelled(
        DeliveryStatus initialStatus)
    {
        var delivery = GetTestDelivery(initialStatus);

        Assert.Throws<InvalidOperationException>(delivery.Postpone);
    }

    [Theory]
    [InlineData(DeliveryStatus.Scheduled)]
    [InlineData(DeliveryStatus.Delivering)]
    [InlineData(DeliveryStatus.Postponed)]
    public void Cancel_ChangesStatusToCancelled_WhenInitialStatusIsNotCompletedNorCancelled(
        DeliveryStatus initialStatus)
    {
        var delivery = GetTestDelivery(initialStatus);

        delivery.Cancel();

        Assert.Equal(DeliveryStatus.Cancelled, delivery.Status);
    }

    [Theory]
    [InlineData(DeliveryStatus.Completed)]
    [InlineData(DeliveryStatus.Cancelled)]
    public void Cancel_ThrowsInvalidOpException_WhenInitialStatusIsCompletedOrCancelled(
        DeliveryStatus initialStatus)
    {
        var delivery = GetTestDelivery(initialStatus);

        Assert.Throws<InvalidOperationException>(delivery.Cancel);
    }
}
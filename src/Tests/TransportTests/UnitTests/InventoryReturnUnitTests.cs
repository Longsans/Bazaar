namespace TransportTests.UnitTests;

public class InventoryReturnUnitTests
{
    private const string _validLotNo = "LOT-1";
    private const uint _validQuantity = 12;
    private const string _validAddress = "308 Negra Arroyo Lane";
    private readonly DateTime _validDeliveryTime = DateTime.Now.AddDays(3);
    private const string _ownerId = "CLNT-1";
    private readonly ReturnQuantity[] _validReturnQuantities = new[]
    {
        new ReturnQuantity("LOT-1", 10),
        new ReturnQuantity("LOT-2", 20),
    };

    private InventoryReturn GetTestReturn(DeliveryStatus? status = null)
    {
        var invenReturn = new InventoryReturn(
            _validAddress, _validReturnQuantities, _validDeliveryTime, _ownerId);
        if (status != null)
        {
            typeof(InventoryReturn).GetProperty(nameof(InventoryReturn.Status))!
                .SetValue(invenReturn, status);
        }
        return invenReturn;
    }

    [Fact]
    public void ReturnQuantity_Constructor_Succeeds_WhenAllValid()
    {
        var returnQuantity = new ReturnQuantity(_validLotNo, _validQuantity);

        Assert.Equal(_validLotNo, returnQuantity.LotNumber);
        Assert.Equal(_validQuantity, returnQuantity.Quantity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void ReturnQuantity_Constructor_ThrowsArgumentException_WhenLotNumberEmpty(string lotNo)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var returnQuantity = new ReturnQuantity(lotNo, _validQuantity);
        });
    }

    [Fact]
    public void ReturnQuantity_Constructor_ThrowsArgOutOfRangeException_WhenQuantityIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var returnQuantity = new ReturnQuantity(_validLotNo, 0u);
        });
    }

    [Fact]
    public void InventoryReturn_Constructor_Succeeds_WhenAllValid()
    {
        var invenReturn = new InventoryReturn(
            _validAddress, _validReturnQuantities, _validDeliveryTime, _ownerId);

        ExtendedAssert.SameTime(DateTime.Now, invenReturn.TimeScheduledAt);
        Assert.Equal(_validDeliveryTime, invenReturn.EstimatedDeliveryTime);
        Assert.Equal(DeliveryStatus.Scheduled, invenReturn.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void InventoryReturn_Constructor_ThrowsArgException_WhenDeliveryAddressEmpty(string address)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var invenReturn = new InventoryReturn(
                address, _validReturnQuantities, _validDeliveryTime, _ownerId);
        });
    }

    [Fact]
    public void InventoryReturn_Constructor_ThrowsArgOutOfRangeException_WhenDeliveryTimeBeforeCurrentDate()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var invenReturn = new InventoryReturn(
                _validAddress, _validReturnQuantities, DateTime.Now.AddSeconds(-1), _ownerId);
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void InventoryReturn_Constructor_ThrowsArgException_WhenInventoryOwnerIdEmpty(string ownerId)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var invenReturn = new InventoryReturn(
                _validAddress, _validReturnQuantities, _validDeliveryTime, ownerId);
        });
    }

    [Fact]
    public void InventoryReturn_Constructor_ThrowsArgException_WhenReturnQuantitiesContainDuplicateLots()
    {
        var returnQuantities = new ReturnQuantity[]
        {
            new("LOT-1", 10),
            new("LOT-1", 20),
        };

        Assert.Throws<ArgumentException>(() =>
        {
            var invenReturn = new InventoryReturn(
                _validAddress, returnQuantities, _validDeliveryTime, _ownerId);
        });
    }

    [Fact]
    public void StartDelivery_SetsStatusToDelivering_WhenStatusIsScheduledOrPostponed()
    {
        var invenReturn = GetTestReturn();

        invenReturn.StartDelivery();

        Assert.Equal(DeliveryStatus.Delivering, invenReturn.Status);
    }

    [Theory]
    [InlineData(DeliveryStatus.Delivering)]
    [InlineData(DeliveryStatus.Completed)]
    [InlineData(DeliveryStatus.Cancelled)]
    public void StartDelivery_ThrowsInvalidOpException_WhenStatusIsNeitherScheduledNorPostponed(
        DeliveryStatus status)
    {
        var invenReturn = GetTestReturn(status);

        Assert.Throws<InvalidOperationException>(invenReturn.StartDelivery);
        if (status != DeliveryStatus.Delivering)
        {
            Assert.NotEqual(DeliveryStatus.Delivering, invenReturn.Status);
        }
    }

    [Fact]
    public void Complete_Succeeds_WhenStatusIsDelivering()
    {
        var invenReturn = GetTestReturn(DeliveryStatus.Delivering);

        invenReturn.Complete();

        Assert.Equal(DeliveryStatus.Completed, invenReturn.Status);
    }

    [Theory]
    [InlineData(DeliveryStatus.Scheduled)]
    [InlineData(DeliveryStatus.Completed)]
    [InlineData(DeliveryStatus.Cancelled)]
    public void Complete_ThrowsInvalidOpException_WhenStatusNotDelivering(
        DeliveryStatus status)
    {
        var invenReturn = GetTestReturn(status);

        Assert.Throws<InvalidOperationException>(invenReturn.Complete);
        if (status != DeliveryStatus.Completed)
        {
            Assert.NotEqual(DeliveryStatus.Completed, invenReturn.Status);
        }
    }

    [Theory]
    [InlineData(DeliveryStatus.Scheduled)]
    [InlineData(DeliveryStatus.Delivering)]
    public void Postpone_Succeeds_WhenStatusIsNeitherCompletedNorCancelledNorPostponed(DeliveryStatus status)
    {
        var invenReturn = GetTestReturn(status);

        invenReturn.Postpone();

        Assert.Equal(DeliveryStatus.Postponed, invenReturn.Status);
    }

    [Theory]
    [InlineData(DeliveryStatus.Postponed)]
    [InlineData(DeliveryStatus.Completed)]
    [InlineData(DeliveryStatus.Cancelled)]
    public void Postpone_ThrowsInvalidOpException_WhenStatusCompletedOrCancelledOrPostponed(
        DeliveryStatus status)
    {
        var invenReturn = GetTestReturn(status);

        Assert.Throws<InvalidOperationException>(invenReturn.Postpone);
        if (status != DeliveryStatus.Postponed)
        {
            Assert.NotEqual(DeliveryStatus.Postponed, invenReturn.Status);
        }
    }

    [Theory]
    [InlineData(DeliveryStatus.Scheduled)]
    [InlineData(DeliveryStatus.Delivering)]
    [InlineData(DeliveryStatus.Postponed)]
    public void Cancel_Succeeds_WhenStatusIsNeitherCompletedNorCancelledAndReasonNotEmpty(DeliveryStatus status)
    {
        var invenReturn = GetTestReturn(status);
        var cancelReason = "Street riot damaged stock.";

        invenReturn.Cancel(cancelReason);

        Assert.Equal(DeliveryStatus.Cancelled, invenReturn.Status);
        Assert.Equal(cancelReason, invenReturn.CancelReason);
    }

    [Theory]
    [InlineData(DeliveryStatus.Completed)]
    [InlineData(DeliveryStatus.Cancelled)]
    public void Cancel_ThrowsInvalidOpException_WhenStatusIsCompletedOrCancelled(DeliveryStatus status)
    {
        var invenReturn = GetTestReturn(status);
        var cancelReason = "Street riot damaged stock.";

        Assert.Throws<InvalidOperationException>(() => invenReturn.Cancel(cancelReason));
        if (status != DeliveryStatus.Cancelled)
        {
            Assert.NotEqual(DeliveryStatus.Cancelled, invenReturn.Status);
            Assert.Null(invenReturn.CancelReason);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Cancel_ThrowsArgumentException_WhenCancelReasonIsEmpty(string cancelReason)
    {
        var invenReturn = GetTestReturn();

        Assert.Throws<ArgumentException>(() => invenReturn.Cancel(cancelReason));
        Assert.NotEqual(DeliveryStatus.Cancelled, invenReturn.Status);
        Assert.Null(invenReturn.CancelReason);
    }
}

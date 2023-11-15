namespace TransportTests.UnitTests;

public class DeliveryPackageItemUnitTests
{
    private const string _validProductId = "PROD-1";
    private const uint _validQuantity = 10;

    [Fact]
    public void Constructor_Succeeds_WhenAllValid()
    {
        var packageItem = new DeliveryPackageItem(_validProductId, _validQuantity);

        Assert.Equal(_validProductId, packageItem.ProductId);
        Assert.Equal(_validQuantity, packageItem.Quantity);
        Assert.Equal(default, packageItem.DeliveryId);
    }

    [Fact]
    public void Constructor_ThrowsArgOutOfRangeException_WhenQuantityIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var packageItem = new DeliveryPackageItem(_validProductId, 0);
        });
    }
}

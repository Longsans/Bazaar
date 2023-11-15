namespace TransportTests.UnitTests;

public class ProductInventoryUnitTests
{
    private const string _validProductId = "PROD-1";
    private const uint _validNumberOfUnits = 100;

    [Fact]
    public void Constructor_SucceedsWithDefaultPickupId_WhenAllValid()
    {
        var inventory = new ProductInventory(_validProductId, _validNumberOfUnits);

        Assert.Equal(_validProductId, inventory.ProductId);
        Assert.Equal(_validNumberOfUnits, inventory.NumberOfUnits);
        Assert.Equal(default, inventory.PickupId);
    }

    [Fact]
    public void Constructor_ThrowsArgOutOfRangeException_WhenNumberOfUnitsIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var inventory = new ProductInventory(_validProductId, 0);
        });
    }
}

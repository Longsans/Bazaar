using Bazaar.FbbInventory.Domain.Entities;
using Bazaar.FbbInventory.Domain.Exceptions;

namespace FbbInventoryTests.UnitTests;

public class ProductInventoryUnitTests
{
    #region Test data and helpers
    private const string _validProductId = "PROD-1";
    private const uint _validRestockThres = 10;
    private const uint _validMaxStockThres = 300;
    private const uint _unitsInStockAvailable = 100;
    private const int _validSellerInvId = 1;

    private static ProductInventory GetTestInventory(uint stock = _unitsInStockAvailable,
        InventoryStatus? statusToSet = null)
    {
        var inventory = new ProductInventory(
            _validProductId, stock,
            _validRestockThres, _validMaxStockThres, _validSellerInvId);

        if (statusToSet != null)
        {
            typeof(ProductInventory).GetProperty(nameof(inventory.Status))!
                .SetValue(inventory, statusToSet);
        }
        return inventory;
    }
    #endregion

    [Fact]
    public void Constructor_SucceedsWithStatusReady_WhenStockHasUnits()
    {
        var inventory = new ProductInventory(
            _validProductId, _unitsInStockAvailable,
            _validRestockThres, _validMaxStockThres, _validSellerInvId);

        Assert.Equal(InventoryStatus.Ready, inventory.Status);
    }

    [Fact]
    public void Constructor_SucceedsWithUnfulfillableStatusAndDate_WhenStockEmpty()
    {
        var inventory = new ProductInventory(
            _validProductId, 0,
            _validRestockThres, _validMaxStockThres, _validSellerInvId);

        Assert.Equal(InventoryStatus.Unfulfillable, inventory.Status);
        Assert.Equal(DateTime.Now.Date, inventory.UnfulfillableSince);
    }

    [Theory]
    [InlineData(_validMaxStockThres + 1, _unitsInStockAvailable)]
    [InlineData(_validRestockThres, _validMaxStockThres + 1)]
    public void Constructor_ThrowsExceedingMaxStockThresholdException_WhenRestockThresOrUnitsInStockExceedsMaxStockThres(
        uint restockThreshold, uint unitsInStock)
    {
        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            var inventory = new ProductInventory(
                _validProductId, unitsInStock,
                restockThreshold, _validMaxStockThres, _validSellerInvId);
        });
    }

    [Fact]
    public void ReduceStock_SucceedsWithNewStockAndNotUnfulfillable_WhenInventoryHasEnoughStock()
    {
        var inventory = GetTestInventory();
        uint originalStock = inventory.UnitsInStock;
        uint reduction = 10;

        inventory.ReduceStock(reduction);

        Assert.Equal(originalStock - reduction, inventory.UnitsInStock);
        Assert.NotEqual(InventoryStatus.Unfulfillable, inventory.Status);
        Assert.Null(inventory.UnfulfillableSince);
    }

    [Fact]
    public void ReduceStock_SucceedsWithNewStockAndUnfulfillableStatus_WhenInventoryIsZeroAfterReduction()
    {
        var inventory = GetTestInventory();

        inventory.ReduceStock(inventory.UnitsInStock);

        Assert.Equal(0u, inventory.UnitsInStock);
        Assert.Equal(InventoryStatus.Unfulfillable, inventory.Status);
        Assert.Equal(DateTime.Now.Date, inventory.UnfulfillableSince);
    }

    [Fact]
    public void ReduceStock_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.ReduceStock(0);
        });
    }

    [Fact]
    public void ReduceStock_ThrowsNotEnoughStockException_WhenInventoryDoesNotHaveEnoughStock()
    {
        var inventory = GetTestInventory();

        Assert.Throws<NotEnoughStockException>(() =>
        {
            inventory.ReduceStock(inventory.UnitsInStock + 1);
        });
    }

    [Theory]
    [InlineData(20)]
    [InlineData(0)]
    public void Restock_SucceedsWithNewStock_WhenUnitsValid(uint initialStock)
    {
        var inventory = GetTestInventory(initialStock);
        uint restockUnits = 100;

        inventory.Restock(restockUnits);

        Assert.Equal(initialStock + restockUnits, inventory.UnitsInStock);
        Assert.Equal(InventoryStatus.Ready, inventory.Status);
    }

    [Fact]
    public void Restock_ThrowsArgOutOfRangeException_WhenUnitsToRestockIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.Restock(0);
        });
    }

    [Fact]
    public void Restock_ThrowsExceedingMaxStockThresholdException_WhenRestockExceedsMaxStockThreshold()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            inventory.Restock(10000);
        });
    }

    [Fact]
    public void MarkAsUnfulfillable_ChangesStatusAndSetsUnfulfillableSince_WhenInitialStatusIsReady()
    {
        var inventory = GetTestInventory();

        inventory.MarkAsUnfulfillable();

        Assert.Equal(InventoryStatus.Unfulfillable, inventory.Status);
        Assert.Equal(DateTime.Now.Date, inventory.UnfulfillableSince);
    }

    [Theory]
    [InlineData(InventoryStatus.Unfulfillable)]
    [InlineData(InventoryStatus.ToBeDisposed)]
    public void MarkAsUnfulfillable_ThrowsInvalidOpException_WhenInitialStatusIsNotReady(
        InventoryStatus initialStatus)
    {
        var inventory = GetTestInventory(statusToSet: initialStatus);

        Assert.Throws<InvalidOperationException>(inventory.MarkAsUnfulfillable);
    }

    [Theory]
    [InlineData(InventoryStatus.Unfulfillable)]
    [InlineData(InventoryStatus.ToBeDisposed)]
    public void MarkToBeDisposed_Succeeds_WhenInitialStatusIsNotReady(
        InventoryStatus initialStatus)
    {
        var inventory = GetTestInventory(statusToSet: initialStatus);

        inventory.MarkToBeDisposed();

        Assert.Equal(InventoryStatus.ToBeDisposed, inventory.Status);
    }

    [Fact]
    public void MarkToBeDisposed_ThrowsInvalidOpException_WhenInitialStatusIsReady()
    {
        var inventory = GetTestInventory();

        Assert.Throws<InvalidOperationException>(inventory.MarkToBeDisposed);
    }
}
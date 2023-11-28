namespace FbbInventoryTests.UnitTests;

public class LotUnitTests
{
    #region Test data and helpers
    private const uint _validUnitsInStock = 100;
    private const uint _validUnitsPendingRemoval = 20;
    private const uint _defectiveUnits = 5;
    private const uint _wdmgUnits = 10;
    private readonly ProductInventory _inventory = new("PROD-1", _validUnitsInStock,
        _defectiveUnits, _wdmgUnits, 10, 1000, 1);

    private Lot GetTestLot()
    {
        return _inventory.FulfillableLots.First();
    }

    private Lot GetTestLotWithPendingRemovalUnits()
    {
        var lot = GetTestLot();
        lot.LabelUnitsInStockForRemoval(_validUnitsPendingRemoval);
        return lot;
    }

    private static void AssertTotalUnits(Lot lot)
    {
        Assert.Equal(lot.UnitsInStock + lot.UnitsPendingRemoval, lot.TotalUnits);
    }
    #endregion

    [Fact]
    public void LotConstructor_Succeeds_WhenAllValid()
    {
        var lot = new Lot(_inventory, _validUnitsInStock);

        Assert.Equal(_validUnitsInStock, lot.UnitsInStock);
        Assert.Equal(0u, lot.UnitsPendingRemoval);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void LotConstructor_ThrowsArgOutOfRangeException_WhenUnitsInStockIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var lot = new Lot(_inventory, 0u);
        });
    }

    [Fact]
    public void FulfillableLotConstructor_Succeeds_WhenAllValid()
    {
        var fulfillableLot = new FulfillableLot(_inventory, _validUnitsInStock);

        Assert.Equal(DateTime.Now.Date, fulfillableLot.DateEnteredStorage);
        Assert.Equal(_validUnitsInStock, fulfillableLot.UnitsInStock);
        Assert.Equal(0u, fulfillableLot.UnitsPendingRemoval);
        AssertTotalUnits(fulfillableLot);
    }

    [Theory]
    [InlineData(UnfulfillableCategory.Defective)]
    [InlineData(UnfulfillableCategory.WarehouseDamaged)]
    [InlineData(UnfulfillableCategory.CustomerDamaged)]
    [InlineData(UnfulfillableCategory.Stranded)]
    public void UnfulfillableLotConstructor_Succeeds_WhenAllValid(
        UnfulfillableCategory category)
    {
        var unfulfillableLot = new UnfulfillableLot(_inventory, _validUnitsInStock, category);

        Assert.Equal(DateTime.Now.Date, unfulfillableLot.DateUnfulfillableSince);
        Assert.Equal(_validUnitsInStock, unfulfillableLot.UnitsInStock);
        Assert.Equal(0u, unfulfillableLot.UnitsPendingRemoval);
        AssertTotalUnits(unfulfillableLot);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100)]
    public void UnfulfillableLotConstructor_ThrowsArgOutOfRangeException_WhenUnfulfillableCategoryDoesNotExist(
        int enumValue)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var unfulfillableLot = new UnfulfillableLot(_inventory,
                _validUnitsInStock, (UnfulfillableCategory)enumValue);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsInStock)]
    public void ReduceStock_ReducesUnitsInStock_WhenAllValid(uint units)
    {
        var lot = GetTestLot();
        uint remainingUnits = lot.UnitsInStock - units;

        lot.ReduceStock(units);

        Assert.Equal(remainingUnits, lot.UnitsInStock);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void ReduceStock_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLot();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.ReduceStock(0u);
        });
    }

    [Fact]
    public void ReduceStock_ThrowsNotEnoughStockException_WhenUnitsInStockFewerThanReduceUnits()
    {
        var lot = GetTestLot();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.ReduceStock(lot.UnitsInStock + 1);
        });
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000 - _validUnitsInStock - _defectiveUnits - _wdmgUnits)]
    public void AddStock_AddsToUnitsInStock_WhenAllValid(uint units)
    {
        var lot = GetTestLot();
        var addedStock = lot.UnitsInStock + units;

        lot.AddStock(units);

        Assert.Equal(addedStock, lot.UnitsInStock);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void AddStock_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLot();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.AddStock(0u);
        });
    }

    [Fact]
    public void AddStock_ThrowsExceedingMaxStockThresholdException_WhenAdditionResultExceedsMaxStockThreshold()
    {
        var lot = GetTestLot();

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            lot.AddStock(_inventory.MaxStockThreshold - lot.UnitsInStock + 1);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsInStock)]
    public void LabelUnitsInStockForRemoval_MovesUnitsInStockToPendingRemoval_WhenAllValid(uint units)
    {
        var lot = GetTestLot();
        var reducedStock = lot.UnitsInStock - units;
        var increasedPendingRemovalUnits = lot.UnitsPendingRemoval + units;

        lot.LabelUnitsInStockForRemoval(units);

        Assert.Equal(reducedStock, lot.UnitsInStock);
        Assert.Equal(increasedPendingRemovalUnits, lot.UnitsPendingRemoval);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void LabelUnitsInStockForRemoval_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLot();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.LabelUnitsInStockForRemoval(0u);
        });
    }

    [Fact]
    public void LabelUnitsInStockForRemoval_ThrowsNotEnoughStockException_WhenUnitsInStockFewerThanUnitsToLabel()
    {
        var lot = GetTestLot();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.LabelUnitsInStockForRemoval(lot.UnitsInStock + 1);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsPendingRemoval)]
    public void RemovePendingUnits_Succeeds_WhenAllValid(uint units)
    {
        var lot = GetTestLotWithPendingRemovalUnits();
        var pendingUnitsAfterRemoval = lot.UnitsPendingRemoval - units;

        lot.RemovePendingUnits(units);

        Assert.Equal(pendingUnitsAfterRemoval, lot.UnitsPendingRemoval);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void RemovePendingUnits_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLotWithPendingRemovalUnits();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.RemovePendingUnits(0u);
        });
    }

    [Fact]
    public void RemovePendingUnits_ThrowsNotEnoughStockException_WhenUnitsPendingRemovalFewerThanUnitsToRemove()
    {
        var lot = GetTestLotWithPendingRemovalUnits();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.RemovePendingUnits(lot.UnitsPendingRemoval + 1);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsPendingRemoval)]
    public void ReturnPendingUnitsToStock_Succeeds_WhenAllValid(uint units)
    {
        var lot = GetTestLotWithPendingRemovalUnits();
        var stockAfterReturn = lot.UnitsInStock + units;
        var pendingUnitsAfterReturn = lot.UnitsPendingRemoval - units;

        lot.ReturnPendingUnitsToStock(units);

        Assert.Equal(stockAfterReturn, lot.UnitsInStock);
        Assert.Equal(pendingUnitsAfterReturn, lot.UnitsPendingRemoval);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void ReturnPendingUnitsToStock_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLotWithPendingRemovalUnits();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.ReturnPendingUnitsToStock(0u);
        });
    }

    [Fact]
    public void ReturnPendingUnitsToStock_ThrowsNotEnoughStockException_WhenUnitsPendingRemovalFewerThanUnitsToReturn()
    {
        var lot = GetTestLotWithPendingRemovalUnits();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.ReturnPendingUnitsToStock(lot.UnitsPendingRemoval + 1);
        });
    }
}

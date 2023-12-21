﻿namespace FbbInventoryTests.UnitTests;

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
        lot.LabelRegularUnitsForRemoval(_validUnitsPendingRemoval);
        return lot;
    }

    private static void AssertTotalUnits(Lot lot)
    {
        Assert.Equal(lot.UnitsInStock + lot.PendingRemovalUnits, lot.TotalUnits);
    }
    #endregion

    [Fact]
    public void FulfillableConstructor_Succeeds_WhenAllValid()
    {
        var lot = new Lot(_inventory, _validUnitsInStock);

        ExtendedAssert.SameTime(lot.DateUnitsEnteredStorage, DateTime.Now);
        Assert.Equal(_validUnitsInStock, lot.UnitsInStock);
        Assert.Equal(0u, lot.PendingRemovalUnits);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void FulfillableConstructor_ThrowsArgOutOfRangeException_WhenUnitsInStockIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var lot = new Lot(_inventory, 0u);
        });
    }

    [Theory]
    [InlineData(UnfulfillableCategory.Defective)]
    [InlineData(UnfulfillableCategory.CustomerDamaged)]
    [InlineData(UnfulfillableCategory.WarehouseDamaged)]
    [InlineData(UnfulfillableCategory.Stranded)]
    public void UnfulfillableConstructor_Succeeds_WhenAllValid(UnfulfillableCategory category)
    {
        var lot = new Lot(_inventory, category, _validUnitsInStock);

        ExtendedAssert.SameTime(lot.DateUnitsEnteredStorage, DateTime.Now);
        Assert.NotNull(lot.DateUnitsBecameUnfulfillable);
        ExtendedAssert.SameTime(lot.DateUnitsBecameUnfulfillable.Value, DateTime.Now);
        Assert.Equal(category, lot.UnfulfillableCategory);
        Assert.Equal(_validUnitsInStock, lot.UnitsInStock);
        Assert.Equal(0u, lot.PendingRemovalUnits);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void UnfulfillableConstructor_ThrowsArgOutOfRangeException_WhenUnfulfillableCategoryNotExist()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var lot = new Lot(_inventory, (UnfulfillableCategory)1111, _validUnitsInStock);
        });
    }

    [Fact]
    public void UnfulfillableConstructor_ThrowsArgOutOfRangeException_WhenUnitsInStockIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var lot = new Lot(_inventory, UnfulfillableCategory.Defective, 0u);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsInStock)]
    public void ReduceStock_ReducesUnitsInStock_WhenAllValid(uint units)
    {
        var lot = GetTestLot();
        uint remainingUnits = lot.UnitsInStock - units;

        lot.ReduceUnits(units);

        Assert.Equal(remainingUnits, lot.UnitsInStock);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void ReduceStock_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLot();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.ReduceUnits(0u);
        });
    }

    [Fact]
    public void ReduceStock_ThrowsNotEnoughStockException_WhenUnitsInStockFewerThanReduceUnits()
    {
        var lot = GetTestLot();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.ReduceUnits(lot.UnitsInStock + 1);
        });
    }

    [Theory]
    [InlineData(100)]
    [InlineData(1000 - _validUnitsInStock - _defectiveUnits - _wdmgUnits)]
    public void AddStock_AddsToUnitsInStock_WhenAllValid(uint units)
    {
        var lot = GetTestLot();
        var addedStock = lot.UnitsInStock + units;

        lot.AddRegularUnits(units);

        Assert.Equal(addedStock, lot.UnitsInStock);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void AddStock_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLot();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.AddRegularUnits(0u);
        });
    }

    [Fact]
    public void AddStock_ThrowsExceedingMaxStockThresholdException_WhenAdditionResultExceedsMaxStockThreshold()
    {
        var lot = GetTestLot();

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            lot.AddRegularUnits(_inventory.MaxStockThreshold - lot.UnitsInStock + 1);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsInStock)]
    public void LabelUnitsInStockForRemoval_MovesUnitsInStockToPendingRemoval_WhenAllValid(uint units)
    {
        var lot = GetTestLot();
        var reducedStock = lot.UnitsInStock - units;
        var increasedPendingRemovalUnits = lot.PendingRemovalUnits + units;

        lot.LabelRegularUnitsForRemoval(units);

        Assert.Equal(reducedStock, lot.UnitsInStock);
        Assert.Equal(increasedPendingRemovalUnits, lot.PendingRemovalUnits);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void LabelUnitsInStockForRemoval_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLot();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.LabelRegularUnitsForRemoval(0u);
        });
    }

    [Fact]
    public void LabelUnitsInStockForRemoval_ThrowsNotEnoughStockException_WhenUnitsInStockFewerThanUnitsToLabel()
    {
        var lot = GetTestLot();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.LabelRegularUnitsForRemoval(lot.UnitsInStock + 1);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsPendingRemoval)]
    public void RemovePendingUnits_Succeeds_WhenAllValid(uint units)
    {
        var lot = GetTestLotWithPendingRemovalUnits();
        var pendingUnitsAfterRemoval = lot.PendingRemovalUnits - units;

        lot.RemovePendingUnits(units);

        Assert.Equal(pendingUnitsAfterRemoval, lot.PendingRemovalUnits);
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
            lot.RemovePendingUnits(lot.PendingRemovalUnits + 1);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsPendingRemoval)]
    public void ReturnPendingUnitsToStock_Succeeds_WhenAllValid(uint units)
    {
        var lot = GetTestLotWithPendingRemovalUnits();
        var stockAfterReturn = lot.UnitsInStock + units;
        var pendingUnitsAfterReturn = lot.PendingRemovalUnits - units;

        lot.RestorePendingUnitsToRegular(units);

        Assert.Equal(stockAfterReturn, lot.UnitsInStock);
        Assert.Equal(pendingUnitsAfterReturn, lot.PendingRemovalUnits);
        AssertTotalUnits(lot);
    }

    [Fact]
    public void ReturnPendingUnitsToStock_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLotWithPendingRemovalUnits();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.RestorePendingUnitsToRegular(0u);
        });
    }

    [Fact]
    public void ReturnPendingUnitsToStock_ThrowsNotEnoughStockException_WhenUnitsPendingRemovalFewerThanUnitsToReturn()
    {
        var lot = GetTestLotWithPendingRemovalUnits();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.RestorePendingUnitsToRegular(lot.PendingRemovalUnits + 1);
        });
    }

    [Theory]
    [InlineData(UnfulfillableCategory.Defective)]
    [InlineData(UnfulfillableCategory.CustomerDamaged)]
    [InlineData(UnfulfillableCategory.WarehouseDamaged)]
    [InlineData(UnfulfillableCategory.Stranded)]
    public void LabelUnfulfillable_SetsUnfulfillableCategoryAndTimeUnfulfillableSinceToNow(
        UnfulfillableCategory category)
    {
        var lot = GetTestLot();

        lot.LabelUnfulfillable(category);

        Assert.NotNull(lot.DateUnitsBecameUnfulfillable);
        ExtendedAssert.SameTime(lot.DateUnitsBecameUnfulfillable.Value, DateTime.Now);
        Assert.Equal(category, lot.UnfulfillableCategory);
    }

    [Fact]
    public void RemoveUnfulfillableLabel_SetsUnfulfillableCategoryAndTimeToNull_WhenCategoryIsStranded()
    {
        var lot = GetTestLot();
        lot.LabelUnfulfillable(UnfulfillableCategory.Stranded);

        lot.RemoveUnfulfillableLabel();

        Assert.Null(lot.DateUnitsBecameUnfulfillable);
        Assert.Null(lot.UnfulfillableCategory);
    }

    [Theory]
    [InlineData(UnfulfillableCategory.Defective)]
    [InlineData(UnfulfillableCategory.CustomerDamaged)]
    [InlineData(UnfulfillableCategory.WarehouseDamaged)]
    public void RemoveUnfulfillableLabel_ThrowsInvalidOpException_WhenCategoryNotStranded(
        UnfulfillableCategory category)
    {
        var lot = GetTestLot();
        lot.LabelUnfulfillable(category);

        Assert.Throws<InvalidOperationException>(lot.RemoveUnfulfillableLabel);
    }
}

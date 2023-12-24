namespace FbbInventoryTests.UnitTests;

public class LotUnitTests
{
    #region Test data and helpers
    private const uint _validUnitsInStock = 100;
    private const uint _validUnitsInRemoval = 20;
    private const uint _defectiveUnits = 5;
    private const uint _wdmgUnits = 10;
    private const uint _maxStockThreshold = 1000;
    private readonly ProductInventory _inventory = new("PROD-1", _validUnitsInStock,
        _defectiveUnits, _wdmgUnits, 10, _maxStockThreshold, 1);

    private Lot GetFulfillableTestLot()
        => _inventory.FulfillableLots.First();

    private Lot GetUnfulfillableTestLot()
        => _inventory.UnfulfillableLots.First();

    private Lot GetStrandedTestLot()
    {
        _inventory.TurnStranded();
        return _inventory.StrandedLots.First();
    }

    private Lot GetTestLotWithUnitsInRemoval()
    {
        var lot = GetFulfillableTestLot();
        lot.IssueUnits(_validUnitsInRemoval, StockIssueReason.Disposal);
        return lot;
    }

    private static void AssertUnits(Lot lot, uint inStock, uint inRemoval)
    {
        Assert.Equal(inStock, lot.UnitsInStock);
        Assert.Equal(inRemoval, lot.UnitsInRemoval);
    }

    private static void AssertStockIssue(StockIssue issue, StockIssueReason issueReason,
        ProductInventory inventory, Lot lot, uint issuedUnits, uint remainingUnits)
    {
        Assert.Equal(issueReason, issue.IssueReason);
        var issuedItem = issue.Items.Single();
        Assert.Equal(inventory.ProductId, issuedItem.ProductId);
        Assert.Equal(issuedUnits, issuedItem.Quantity);
        Assert.Equal(remainingUnits, lot.UnitsInStock);
    }
    #endregion

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-30)]
    public void GoodConstructor_ConstructsFulfillableLot_WhenDateStrandedIsNull(int daysFromNow)
    {
        var dateEnteredStorage = DateTime.Now.Date.AddDays(daysFromNow);
        var lot = new Lot(_inventory, _validUnitsInStock, dateEnteredStorage);

        Assert.Equal(dateEnteredStorage, lot.DateUnitsEnteredStorage);
        Assert.True(lot.IsUnitsFulfillable);
        AssertUnits(lot, _validUnitsInStock, 0u);
    }

    [Fact]
    public void GoodConstructor_ConstructsFulfillableLotEnteredStorageOnCurrentDate_WhenNoDateSpecified()
    {
        var lot = new Lot(_inventory, _validUnitsInStock);

        Assert.Equal(DateTime.Now.Date, lot.DateUnitsEnteredStorage);
        Assert.True(!lot.IsUnitsUnfulfillable && !lot.IsUnitsStranded);
        AssertUnits(lot, _validUnitsInStock, 0u);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-30)]
    public void GoodConstructor_ConstructsStrandedLot_WhenDateStrandedIsNotNull(int daysFromNow)
    {
        var dateEnteredStorage = DateTime.Now.Date.AddDays(daysFromNow);
        var dateStranded = dateEnteredStorage;
        var lot = new Lot(_inventory, _validUnitsInStock, dateEnteredStorage, dateStranded);

        Assert.Equal(dateEnteredStorage, lot.DateUnitsEnteredStorage);
        AssertUnits(lot, _validUnitsInStock, 0u);
    }

    [Fact]
    public void GoodConstructor_ThrowsArgOutOfRangeException_WhenUnitsInStockIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var lot = new Lot(_inventory, 0u);
        });
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void GoodConstructor_ThrowsArgOutOfRangeException_WhenAnyEventDateIsInFuture(
        bool dateEnteredStorageInFuture, bool dateStrandedInFuture)
    {
        var dateEnteredStorage = dateEnteredStorageInFuture ? DateTime.Now.Date.AddDays(1) : DateTime.Now.Date;
        var dateStranded = dateStrandedInFuture ? DateTime.Now.Date.AddDays(1) : DateTime.Now.Date;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var lot = new Lot(_inventory, _validUnitsInStock, dateEnteredStorage, dateStranded);
        });
    }

    [Fact]
    public void GoodConstructor_ThrowsArgException_WhenStrandedBeforeEnteredStorage()
    {
        var dateEnteredStorage = DateTime.Now.Date;
        var dateStranded = dateEnteredStorage.AddDays(-1);
        Assert.Throws<ArgumentException>(() =>
        {
            var lot = new Lot(_inventory, _validUnitsInStock, dateEnteredStorage, dateStranded);
        });
    }

    [Theory]
    [InlineData(UnfulfillableCategory.Defective)]
    [InlineData(UnfulfillableCategory.CustomerDamaged)]
    [InlineData(UnfulfillableCategory.WarehouseDamaged)]
    public void UnfulfillableConstructor_Succeeds_WhenAllValid(UnfulfillableCategory category)
    {
        var dateEnteredStorage = DateTime.Now.Date;
        var dateUnfulfillable = dateEnteredStorage;
        var lot = new Lot(_inventory, _validUnitsInStock, dateEnteredStorage, dateUnfulfillable, category);

        Assert.Equal(DateTime.Now.Date, lot.DateUnitsEnteredStorage);
        Assert.Equal(DateTime.Now.Date, lot.DateUnitsBecameUnfulfillable!);
        Assert.Equal(category, lot.UnfulfillableCategory);
        AssertUnits(lot, _validUnitsInStock, 0u);
    }

    [Fact]
    public void UnfulfillableConstructor_ThrowsArgOutOfRangeException_WhenDateUnfulfillableInFuture()
    {
        var dateBecameUnfulfillable = DateTime.Now.Date.AddDays(1);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var lot = new Lot(_inventory, _validUnitsInStock, DateTime.Now.Date,
                dateBecameUnfulfillable, UnfulfillableCategory.Defective);
        });
    }

    [Fact]
    public void UnfulfillableConstructor_ThrowsArgException_WhenUnfulfillableBeforeEnteredStorage()
    {
        var dateEnteredStorage = DateTime.Now.Date;
        var dateBecameUnfulfillable = dateEnteredStorage.AddDays(-1);
        Assert.Throws<ArgumentException>(() =>
        {
            var lot = new Lot(_inventory, _validUnitsInStock, dateEnteredStorage,
                dateBecameUnfulfillable, UnfulfillableCategory.Defective);
        });
    }

    [Fact]
    public void UnfulfillableConstructor_ThrowsArgOutOfRangeException_WhenUnfulfillableCategoryNotExist()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var lot = new Lot(_inventory, _validUnitsInStock, (UnfulfillableCategory)1111);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsInStock)]
    public void IssueUnits_ForSale_ReducesUnitsInStockAndReturnsIssue_WhenUnitsFulfillableAndEnoughStock(uint units)
    {
        var lot = GetFulfillableTestLot();
        var issueReason = StockIssueReason.Sale;
        var remainingStock = lot.UnitsInStock - units;

        var issue = lot.IssueUnits(units, issueReason);

        AssertStockIssue(issue, issueReason, _inventory, lot, units, remainingStock);
    }

    [Fact]
    public void IssueUnits_ForSale_ThrowsInvalidOpException_WhenUnitsUnfulfillable()
    {
        var units = 10u;
        var lot = GetUnfulfillableTestLot();

        Assert.Throws<InvalidOperationException>(() =>
        {
            var issue = lot.IssueUnits(units, StockIssueReason.Sale);
        });
    }

    [Fact]
    public void IssueUnits_ForSale_ThrowsInvalidOpException_WhenUnitsStranded()
    {
        var units = 10u;
        var lot = GetStrandedTestLot();

        Assert.Throws<InvalidOperationException>(() =>
        {
            var issue = lot.IssueUnits(units, StockIssueReason.Sale);
        });
    }

    [Theory]
    [InlineData(10, StockIssueReason.Return)]
    [InlineData(10, StockIssueReason.Disposal)]
    [InlineData(_validUnitsInStock, StockIssueReason.Return)]
    [InlineData(_validUnitsInStock, StockIssueReason.Disposal)]
    public void IssueUnits_ForRemoval_MovesUnitsFromStockToRemovalAndReturnsIssue_WhenEnoughStock(
        uint units, StockIssueReason issueReason)
    {
        var lot = GetFulfillableTestLot();
        var remainingStock = lot.UnitsInStock - units;
        var resultingUnitsInRemoval = lot.UnitsInRemoval + units;

        var issue = lot.IssueUnits(units, issueReason);

        AssertStockIssue(issue, issueReason, _inventory, lot, units, remainingStock);
        Assert.Equal(resultingUnitsInRemoval, lot.UnitsInRemoval);
    }

    [Fact]
    public void IssueUnits_ThrowsArgOutOfRangeException_WhenIssueReasonNotExist()
    {
        var units = 10u;
        var lot = GetFulfillableTestLot();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var issue = lot.IssueUnits(units, (StockIssueReason)1111);
        });
    }

    [Theory]
    [InlineData(100)]
    [InlineData(_maxStockThreshold - _validUnitsInStock - _defectiveUnits - _wdmgUnits)]
    [InlineData(-10)]
    [InlineData(-(int)_validUnitsInStock)]
    public void AdjustUnits_AddsToUnitsInStock_WhenAllValid(int units)
    {
        var lot = GetFulfillableTestLot();
        var adjustedUnits = lot.UnitsInStock + units;

        lot.AdjustUnits(units);

        Assert.Equal(adjustedUnits, lot.UnitsInStock);
    }

    [Fact]
    public void AdjustUnits_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetFulfillableTestLot();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.AdjustUnits(0);
        });
    }

    [Fact]
    public void AdjustUnits_ThrowsExceedingMaxStockThresholdException_WhenAdditionExceedsMaxStockThreshold()
    {
        var lot = GetFulfillableTestLot();

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            lot.AdjustUnits((int)(_inventory.MaxStockThreshold - lot.UnitsInStock + 1));
        });
    }

    [Fact]
    public void AdjustUnits_ThrowsNotEnoughUnitsException_WhenNotEnoughUnits()
    {
        var lot = GetFulfillableTestLot();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.AdjustUnits(-(int)(lot.UnitsInStock + 1));
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsInRemoval)]
    public void ConfirmUnitsRemoved_Succeeds_WhenAllValid(uint units)
    {
        var lot = GetTestLotWithUnitsInRemoval();
        var remainingUnitsInRemoval = lot.UnitsInRemoval - units;

        lot.ConfirmUnitsRemoved(units);

        Assert.Equal(remainingUnitsInRemoval, lot.UnitsInRemoval);
    }

    [Fact]
    public void ConfirmUnitsRemoved_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLotWithUnitsInRemoval();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.ConfirmUnitsRemoved(0u);
        });
    }

    [Fact]
    public void ConfirmUnitsRemoved_ThrowsNotEnoughStockException_WhenUnitsInRemovalFewerThanBeingConfirmed()
    {
        var lot = GetTestLotWithUnitsInRemoval();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.ConfirmUnitsRemoved(lot.UnitsInRemoval + 1);
        });
    }

    [Theory]
    [InlineData(10)]
    [InlineData(_validUnitsInRemoval)]
    public void RestoreUnitsFromRemoval_Succeeds_WhenAllValid(uint units)
    {
        var lot = GetTestLotWithUnitsInRemoval();
        var stockAfterReturn = lot.UnitsInStock + units;
        var pendingUnitsAfterReturn = lot.UnitsInRemoval - units;

        lot.RestoreUnitsFromRemoval(units);

        Assert.Equal(stockAfterReturn, lot.UnitsInStock);
        Assert.Equal(pendingUnitsAfterReturn, lot.UnitsInRemoval);
    }

    [Fact]
    public void RestoreUnitsFromRemoval_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var lot = GetTestLotWithUnitsInRemoval();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            lot.RestoreUnitsFromRemoval(0u);
        });
    }

    [Fact]
    public void RestoreUnitsFromRemoval_ThrowsNotEnoughStockException_WhenUnitsInRemovalFewerThanBeingRestored()
    {
        var lot = GetTestLotWithUnitsInRemoval();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            lot.RestoreUnitsFromRemoval(lot.UnitsInRemoval + 1);
        });
    }

    [Fact]
    public void TurnStranded_SetsDateStrandedToCurrentDate_WhenNotStranded()
    {
        var lot = GetFulfillableTestLot();

        lot.TurnStranded();

        Assert.True(lot.IsUnitsStranded);
        Assert.Equal(DateTime.Now.Date, lot.DateUnitsBecameStranded);
    }

    [Fact]
    public void TurnStranded_ThrowsInvalidOpException_WhenUnfulfillable()
    {
        var lot = GetUnfulfillableTestLot();

        Assert.Throws<InvalidOperationException>(lot.TurnStranded);
    }

    [Fact]
    public void TurnStranded_ThrowsInvalidOpException_WhenAlreadyStranded()
    {
        var lot = GetStrandedTestLot();

        Assert.Throws<InvalidOperationException>(lot.TurnStranded);
    }

    [Fact]
    public void ConfirmStrandingResolved_SetsDateStrandedToNull_WhenStranded()
    {
        var lot = GetStrandedTestLot();

        lot.ConfirmStrandingResolved();

        Assert.False(lot.IsUnitsStranded);
        Assert.Null(lot.DateUnitsBecameStranded);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ConfirmStrandingResolved_ThrowsInvalidOpException_WhenNotStranded(bool unitsUnfulfillable)
    {
        var lot = unitsUnfulfillable ? GetUnfulfillableTestLot() : GetFulfillableTestLot();

        Assert.Throws<InvalidOperationException>(lot.ConfirmStrandingResolved);
    }
}

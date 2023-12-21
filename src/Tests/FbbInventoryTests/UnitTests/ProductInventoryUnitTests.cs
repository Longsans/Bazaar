namespace FbbInventoryTests.UnitTests;

public class ProductInventoryUnitTests
{
    #region Test data and helpers
    private const string _validProductId = "PROD-1";
    private const uint _validRestockThres = 10;
    private const uint _validMaxStockThres = 1000;
    private const uint _validFulfillableUnits = 100;
    private const uint _defectiveUnits = 5;
    private const uint _wdmgUnits = 10;
    private const int _validSellerInvId = 1;

    private static ProductInventory GetTestInventory()
    {
        var inventory = new ProductInventory(_validProductId, _validFulfillableUnits, _defectiveUnits,
            _wdmgUnits, _validRestockThres, _validMaxStockThres, _validSellerInvId);

        var fLots = GetLotsForProductInventory(inventory);
        var fLot = new Lot(inventory, 50);
        var fLot2 = new Lot(inventory, 75);
        AddDaysToLotEventDate(fLot, -3);
        AddDaysToLotEventDate(fLot2, -2);
        fLots.Add(fLot);
        fLots.Add(fLot2);

        var uLots = GetLotsForProductInventory(inventory);
        var uLot = new Lot(inventory, UnfulfillableCategory.CustomerDamaged, 20);
        var uLot2 = new Lot(inventory, UnfulfillableCategory.CustomerDamaged, 10);
        AddDaysToLotEventDate(uLot, -3);
        AddDaysToLotEventDate(uLot2, -2);
        uLots.Add(uLot);
        uLots.Add(uLot2);

        return inventory;
    }

    private static ProductInventory GetTestInventoryWithoutLotForCurrentDate()
    {
        var inventory = new ProductInventory(_validProductId, _validFulfillableUnits, _defectiveUnits,
            _wdmgUnits, _validRestockThres, _validMaxStockThres, _validSellerInvId);

        var fCurrentDateLot = GetLotsForProductInventory(inventory).Single(x => !x.IsUnitsUnfulfillable);
        AddDaysToLotEventDate(fCurrentDateLot, -3);

        var uCurrentDateLot = GetLotsForProductInventory(inventory)
            .Where(u => u.DateUnitsBecameUnfulfillable?.Date == DateTime.Now.Date)
            .Select(u =>
            {
                AddDaysToLotEventDate(u, -3);
                return u;
            })
            .ToList();

        return inventory;
    }

    private static ProductInventory GetTestInventoryWithStrandedLots()
    {
        var inventory = GetTestInventory();
        foreach (var lot in inventory.UnfulfillableLots
            .Where(x => x.UnfulfillableCategory == UnfulfillableCategory.CustomerDamaged))
        {
            var categoryProperty = typeof(Lot).GetProperty(nameof(Lot.UnfulfillableCategory));
            categoryProperty!.SetValue(lot, UnfulfillableCategory.Stranded);
        }
        return inventory;
    }

    private static List<Lot> GetLotsForProductInventory(ProductInventory inventory)
    {
        return (List<Lot>)typeof(ProductInventory)
            .GetField("_lots", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(inventory)!;
    }

    private static void AddDaysToLotEventDate(Lot lot, int days)
    {
        var eventTimeProperty = typeof(Lot).GetProperty(
            lot.DateUnitsBecameUnfulfillable == null
            ? nameof(lot.DateUnitsEnteredStorage)
            : nameof(lot.DateUnitsBecameUnfulfillable))!;
        eventTimeProperty.SetValue(lot, DateTime.Now.AddDays(days));
    }
    #endregion

    [Fact]
    public void Constructor_Succeeds_WhenAllValid()
    {
        var inventory = new ProductInventory(_validProductId, _validFulfillableUnits, _defectiveUnits,
            _wdmgUnits, _validRestockThres, _validMaxStockThres, _validSellerInvId);

        Assert.Equal(_validFulfillableUnits, inventory.FulfillableUnits);
        Assert.Equal(_defectiveUnits + _wdmgUnits, inventory.UnfulfillableUnits);
        Assert.Equal(_validFulfillableUnits + _defectiveUnits + _wdmgUnits, inventory.TotalUnits);
    }

    [Fact]
    public void Constructor_ThrowsArgOutOfRangeException_WhenTotalUnitsIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var inventory = new ProductInventory(_validProductId, 0u, 0u, 0u,
                _validMaxStockThres, _validMaxStockThres, _validSellerInvId);
        });
    }

    [Fact]
    public void Constructor_ThrowsExceedingMaxStockException_WhenRestockThreshLargerThanMaxStockThresh()
    {
        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            var inventory = new ProductInventory(_validProductId, _validFulfillableUnits, _defectiveUnits,
                _wdmgUnits, _validMaxStockThres + 1, _validMaxStockThres, _validSellerInvId);
        });
    }

    [Fact]
    public void Constructor_ThrowsExceedingMaxStockException_WhenTotalUnitsExceedsMaxStockThreshold()
    {
        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            var inventory = new ProductInventory(_validProductId, 500, 500,
            500, _validRestockThres, _validMaxStockThres, _validSellerInvId);
        });
    }

    // FULFILLABLE REDUCTION
    [Theory]
    [InlineData(25)]
    [InlineData(75)]
    [InlineData(150)]
    public void ReduceFulfillableStockFromOldToNew_ReducesStockAndRemovesEmptyLots_WhenAllValid(uint units)
    {
        var inventory = GetTestInventory();
        var fulfillableUnitsAfterReduction = inventory.FulfillableUnits - units;
        var lotsFromOldToNew = inventory.FulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.DateUnitsEnteredStorage)
            .ToList();

        inventory.IssueFulfillableStockByDateCreated(units);

        Assert.Equal(fulfillableUnitsAfterReduction, inventory.FulfillableUnits);

        var removedLots = lotsFromOldToNew.Except(inventory.FulfillableLots);
        Assert.True(!removedLots.Any() || removedLots.All(
            x => inventory.FulfillableLots.All(f => f.DateUnitsEnteredStorage >= x.DateUnitsEnteredStorage)));
    }

    [Fact]
    public void ReduceFulfillableStockFromOldToNew_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.IssueFulfillableStockByDateCreated(0u);
        });
    }

    [Fact]
    public void ReduceFulfillableStockFromOldToNew_ThrowsNotEnoughUnitsException_WhenUnitsToReduceIsMoreThanFulfillableUnits()
    {
        var inventory = GetTestInventory();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            inventory.IssueFulfillableStockByDateCreated(inventory.FulfillableUnits + 1);
        });
    }

    // UNFULFILLABLE REDUCTION
    [Theory]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(35)]
    public void ReduceUnfulfillableStockFromOldToNew_ReducesStockAndRemovesEmptyLots_WhenAllValid(uint units)
    {
        var inventory = GetTestInventory();
        var unfulfillableUnitsAfterReduction = inventory.UnfulfillableUnits - units;
        var lotsFromOldToNew = inventory.UnfulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.DateUnitsBecameUnfulfillable)
            .ToList();

        inventory.IssueUnfulfillableStockByDateCreated(units);

        Assert.Equal(unfulfillableUnitsAfterReduction, inventory.UnfulfillableUnits);

        var removedLots = lotsFromOldToNew.Except(inventory.UnfulfillableLots);
        Assert.True(!removedLots.Any() || removedLots.All(
            x => inventory.UnfulfillableLots.All(f => f.DateUnitsBecameUnfulfillable >= x.DateUnitsBecameUnfulfillable)));
    }

    [Fact]
    public void ReduceUnfulfillableStockFromOldToNew_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.IssueUnfulfillableStockByDateCreated(0u);
        });
    }

    [Fact]
    public void ReduceUnfulfillableStockFromOldToNew_ThrowsNotEnoughUnitsException_WhenUnitsToReduceIsMoreThanUnfulfillableUnits()
    {
        var inventory = GetTestInventory();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            inventory.IssueUnfulfillableStockByDateCreated(inventory.UnfulfillableUnits + 1);
        });
    }

    // FULFILLABLE
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddFulfillableStock_AddsStockToExistingLotOrCreateNewLotIfNotExists_WhenAllValid(
        bool lotExists)
    {
        var inventory = lotExists ? GetTestInventory()
            : GetTestInventoryWithoutLotForCurrentDate();
        uint units = 100;
        var lotCountAfterAddition = inventory.FulfillableLots.Count + (lotExists ? 0 : 1);
        var fulfillableUnitsInStockAfterAddition = inventory.FulfillableUnits + units;

        inventory.AddFulfillableStock(units);

        Assert.Equal(fulfillableUnitsInStockAfterAddition, inventory.FulfillableUnits);
        Assert.Equal(lotCountAfterAddition, inventory.FulfillableLots.Count);
    }

    [Fact]
    public void AddFulfillableStock_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.AddFulfillableStock(0u);
        });
    }

    [Fact]
    public void AddFulfillableStock_ThrowsExceedingMaxStockThresholdException_WhenAdditionsResultExceedsMaxStockThreshold()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            inventory.AddFulfillableStock(inventory.RemainingCapacity + 1);
        });
    }

    // UNFULFILLABLE
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddUnfulfillableStock_AddsStockToExistingLotOrCreateNewLotIfNotExists_WhenAllValid(
        bool lotExists)
    {
        var inventory = lotExists ? GetTestInventory()
            : GetTestInventoryWithoutLotForCurrentDate();
        uint units = 100;
        var lotCountAfterAddition = inventory.UnfulfillableLots.Count + (lotExists ? 0 : 1);
        var unfulfillableUnitsInStockAfterAddition = inventory.UnfulfillableUnits + units;

        inventory.AddUnfulfillableStock(UnfulfillableCategory.Defective, units);

        Assert.Equal(unfulfillableUnitsInStockAfterAddition, inventory.UnfulfillableUnits);
        Assert.Equal(lotCountAfterAddition, inventory.UnfulfillableLots.Count);
    }

    [Fact]
    public void AddUnfulfillableStock_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.AddUnfulfillableStock(UnfulfillableCategory.Defective, 0u);
        });
    }

    [Fact]
    public void AddUnfulfillableStock_ThrowsExceedingMaxStockThresholdException_WhenAdditionsResultExceedsMaxStockThreshold()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            inventory.AddUnfulfillableStock(UnfulfillableCategory.Defective,
                inventory.RemainingCapacity + 1);
        });
    }

    [Fact]
    public void LabelAllFulfillableStockAsStrandedStock_AlwaysSucceeds()
    {
        var inventory = GetTestInventory();
        var initialFulfillableLots = inventory.FulfillableLots.ToList();

        inventory.TurnStranded();

        Assert.True(inventory.FulfillableUnits + inventory.FulfillablePendingRemovalUnits == 0u);
        Assert.DoesNotContain(initialFulfillableLots, lot =>
            lot.UnfulfillableCategory != UnfulfillableCategory.Stranded);
    }

    [Fact]
    public void RelabelStrandedStockAsFulfillableStock_AlwaysSucceeds()
    {
        var inventory = GetTestInventoryWithStrandedLots();
        var initialStrandedLots = inventory.UnfulfillableLots
            .Where(x => x.UnfulfillableCategory == UnfulfillableCategory.Stranded)
            .ToList();

        inventory.RestoreStrandedStockAsFulfillableStock();

        Assert.DoesNotContain(inventory.Lots, lot =>
            lot.UnfulfillableCategory == UnfulfillableCategory.Stranded);
        Assert.DoesNotContain(initialStrandedLots, lot =>
            lot.UnfulfillableCategory != null || lot.DateUnitsUnfulfillableSince != null);
    }

    [Fact]
    public void RemoveEmptyLots_AlwaysSucceeds()
    {
        var inventory = GetTestInventory();
        var lotsToRemove = new List<Lot>
        {
            inventory.Lots.First(x => !x.IsUnitsUnfulfillable),
            inventory.Lots.First(x => x.IsUnitsUnfulfillable),
        };
        foreach (var lot in lotsToRemove)
        {
            if (lot.UnitsInStock > 0)
                lot.ReduceUnits(lot.UnitsInStock);
            if (lot.PendingRemovalUnits > 0)
                lot.RemovePendingUnits(lot.PendingRemovalUnits);
        }

        inventory.RemoveEmptyLots();

        Assert.DoesNotContain(inventory.Lots, lot => lot.TotalUnits == 0);
        Assert.DoesNotContain(inventory.Lots, lot => lotsToRemove.Contains(lot));
    }
}
using System.Reflection;

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

        var fLots = GetLotsForProductInventory<FulfillableLot>(inventory);
        var fLot = new FulfillableLot(inventory, 50);
        var fLot2 = new FulfillableLot(inventory, 75);
        AddDaysToLotEventDate(fLot, -3);
        AddDaysToLotEventDate(fLot2, -2);
        fLots.Add(fLot);
        fLots.Add(fLot2);

        var uLots = GetLotsForProductInventory<UnfulfillableLot>(inventory);
        var uLot = new UnfulfillableLot(inventory, 20, UnfulfillableCategory.CustomerDamaged);
        var uLot2 = new UnfulfillableLot(inventory, 10, UnfulfillableCategory.CustomerDamaged);
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

        var fCurrentDateLot = GetLotsForProductInventory<FulfillableLot>(inventory).Single();
        AddDaysToLotEventDate(fCurrentDateLot, -3);

        var uCurrentDateLot = GetLotsForProductInventory<UnfulfillableLot>(inventory)
            .Where(u => u.DateUnfulfillableSince == DateTime.Now.Date)
            .Select(u =>
            {
                AddDaysToLotEventDate(u, -3);
                return u;
            })
            .ToList();

        return inventory;
    }

    private static List<T> GetLotsForProductInventory<T>(ProductInventory inventory)
        where T : Lot
    {
        var fieldName = typeof(T) == typeof(FulfillableLot)
            ? "_fulfillableLots"
            : "_unfulfillableLots";

        return (List<T>)typeof(ProductInventory)
            .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(inventory)!;
    }

    private static void AddDaysToLotEventDate<T>(T lot, int days)
        where T : Lot
    {
        var eventDatePropName = lot switch
        {
            FulfillableLot => nameof(FulfillableLot.DateEnteredStorage),
            UnfulfillableLot => nameof(UnfulfillableLot.DateUnfulfillableSince)
        };

        typeof(T).GetProperty(eventDatePropName)!
            .SetValue(lot, DateTime.Now.AddDays(days));
    }
    #endregion

    [Fact]
    public void Constructor_Succeeds_WhenAllValid()
    {
        var inventory = new ProductInventory(_validProductId, _validFulfillableUnits, _defectiveUnits,
            _wdmgUnits, _validRestockThres, _validMaxStockThres, _validSellerInvId);

        Assert.Equal(_validFulfillableUnits, inventory.FulfillableUnitsInStock);
        Assert.Equal(_defectiveUnits + _wdmgUnits, inventory.UnfulfillableUnitsInStock);
        Assert.Equal(_validFulfillableUnits + _defectiveUnits + _wdmgUnits, inventory.TotalUnits);
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
        var fulfillableUnitsAfterReduction = inventory.FulfillableUnitsInStock - units;
        var lotsFromOldToNew = inventory.FulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.DateEnteredStorage)
            .ToList();

        inventory.ReduceFulfillableStockFromOldToNew(units);

        Assert.Equal(fulfillableUnitsAfterReduction, inventory.FulfillableUnitsInStock);

        var removedLots = lotsFromOldToNew.Except(inventory.FulfillableLots);
        Assert.True(!removedLots.Any() || removedLots.All(
            x => inventory.FulfillableLots.All(f => f.DateEnteredStorage >= x.DateEnteredStorage)));
    }

    [Fact]
    public void ReduceFulfillableStockFromOldToNew_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.ReduceFulfillableStockFromOldToNew(0u);
        });
    }

    [Fact]
    public void ReduceFulfillableStockFromOldToNew_ThrowsNotEnoughUnitsException_WhenUnitsToReduceIsMoreThanFulfillableUnits()
    {
        var inventory = GetTestInventory();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            inventory.ReduceFulfillableStockFromOldToNew(inventory.FulfillableUnitsInStock + 1);
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
        var unfulfillableUnitsAfterReduction = inventory.UnfulfillableUnitsInStock - units;
        var lotsFromOldToNew = inventory.UnfulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.DateUnfulfillableSince)
            .ToList();

        inventory.ReduceUnfulfillableStockFromOldToNew(units);

        Assert.Equal(unfulfillableUnitsAfterReduction, inventory.UnfulfillableUnitsInStock);

        var removedLots = lotsFromOldToNew.Except(inventory.UnfulfillableLots);
        Assert.True(!removedLots.Any() || removedLots.All(
            x => inventory.UnfulfillableLots.All(f => f.DateUnfulfillableSince >= x.DateUnfulfillableSince)));
    }

    [Fact]
    public void ReduceUnfulfillableStockFromOldToNew_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.ReduceUnfulfillableStockFromOldToNew(0u);
        });
    }

    [Fact]
    public void ReduceUnfulfillableStockFromOldToNew_ThrowsNotEnoughUnitsException_WhenUnitsToReduceIsMoreThanUnfulfillableUnits()
    {
        var inventory = GetTestInventory();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            inventory.ReduceUnfulfillableStockFromOldToNew(inventory.UnfulfillableUnitsInStock + 1);
        });
    }

    // FULFILLABLE LABELING
    [Fact]
    public void LabelFulfillableUnitsFromOldToNewForRemoval_MovesLotUnitsToPendingRemoval_WhenAllValid()
    {
        var inventory = GetTestInventory();
        uint units = 150;
        uint remainingUnitsInStock = inventory.FulfillableUnitsInStock - units;
        uint pendingRemovalUnitsAfterLabeling = inventory.FulfillableUnitsPendingRemoval + units;

        inventory.LabelFulfillableUnitsFromOldToNewForRemoval(units);

        Assert.Equal(remainingUnitsInStock, inventory.FulfillableUnitsInStock);
        Assert.Equal(pendingRemovalUnitsAfterLabeling, inventory.FulfillableUnitsPendingRemoval);
    }

    [Fact]
    public void LabelFulfillableUnitsFromOldToNewForRemoval_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.LabelFulfillableUnitsFromOldToNewForRemoval(0u);
        });
    }

    [Fact]
    public void LabelFulfillableUnitsFromOldToNewForRemoval_ThrowsNotEnoughUnitsException_WhenUnitsToLabelIsMoreThanFulfillableUnits()
    {
        var inventory = GetTestInventory();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            inventory.LabelFulfillableUnitsFromOldToNewForRemoval(inventory.FulfillableUnitsInStock + 1);
        });
    }

    // UNFULFILLABLE LABELING
    [Fact]
    public void LabelUnfulfillableUnitsFromOldToNewForRemoval_MovesLotUnitsToPendingRemoval_WhenAllValid()
    {
        var inventory = GetTestInventory();
        uint units = 35;
        uint remainingUnitsInStock = inventory.UnfulfillableUnitsInStock - units;
        uint pendingRemovalUnitsAfterLabeling = inventory.UnfulfillableUnitsPendingRemoval + units;

        inventory.LabelUnfulfillableUnitsFromOldToNewForRemoval(units);

        Assert.Equal(remainingUnitsInStock, inventory.UnfulfillableUnitsInStock);
        Assert.Equal(pendingRemovalUnitsAfterLabeling, inventory.UnfulfillableUnitsPendingRemoval);
    }

    [Fact]
    public void LabelUnfulfillableUnitsFromOldToNewForRemoval_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.LabelUnfulfillableUnitsFromOldToNewForRemoval(0u);
        });
    }

    [Fact]
    public void LabelUnfulfillableUnitsFromOldToNewForRemoval_ThrowsNotEnoughUnitsException_WhenUnitsToLabelIsMoreThanUnfulfillableUnits()
    {
        var inventory = GetTestInventory();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            inventory.LabelUnfulfillableUnitsFromOldToNewForRemoval(inventory.UnfulfillableUnitsInStock + 1);
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
        var fulfillableUnitsInStockAfterAddition = inventory.FulfillableUnitsInStock + units;

        inventory.AddFulfillableStock(units);

        Assert.Equal(fulfillableUnitsInStockAfterAddition, inventory.FulfillableUnitsInStock);
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
        var unfulfillableUnitsInStockAfterAddition = inventory.UnfulfillableUnitsInStock + units;

        inventory.AddUnfulfillableStock(UnfulfillableCategory.Defective, units);

        Assert.Equal(unfulfillableUnitsInStockAfterAddition, inventory.UnfulfillableUnitsInStock);
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
}
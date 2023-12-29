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
        var threeDaysAgo = DateTime.Now.Date.AddDays(-3);
        var twoDaysAgo = DateTime.Now.Date.AddDays(-2);

        var lots = GetLotsForProductInventory(inventory);
        var fLot = new Lot(inventory, 50, threeDaysAgo);
        var fLot2 = new Lot(inventory, 75, twoDaysAgo);
        var uLot = new Lot(inventory, 5, threeDaysAgo, threeDaysAgo, UnfulfillableCategory.CustomerDamaged);
        var uLot2 = new Lot(inventory, 10, twoDaysAgo, twoDaysAgo, UnfulfillableCategory.CustomerDamaged);
        lots.Add(fLot);
        lots.Add(fLot2);
        lots.Add(uLot);
        lots.Add(uLot2);

        return inventory;
    }

    private static ProductInventory GetTestInventoryWithoutLotForCurrentDate()
    {
        var inventory = new ProductInventory(_validProductId, _validFulfillableUnits, _defectiveUnits,
            _wdmgUnits, _validRestockThres, _validMaxStockThres, _validSellerInvId);
        var lots = GetLotsForProductInventory(inventory);

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

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(150)]
    [InlineData(200)]
    [InlineData(225)]
    public void GetGoodLotsFifoForStockDemand_ReturnsOldestLotsWithEnoughUnits_WhenUnitsFulfillable(uint requiredUnits)
    {
        var inventory = GetTestInventory();

        var lots = inventory.GetGoodLotsFifoForStockDemand(requiredUnits);

        var remainingLots = inventory.FulfillableLots.Except(lots);
        var lotUnits = (uint)lots.Sum(x => x.UnitsInStock);
        var lastLot = lots.TakeLast(1).Single();
        var lotUnitsExcludingLast = lotUnits - lastLot.UnitsInStock;
        Assert.True(lots.All(x => x.IsUnitsFulfillable));
        Assert.True(remainingLots.All(x => lots.All(
            l => l.DateUnitsEnteredStorage <= x.DateUnitsEnteredStorage)));
        Assert.True(lotUnits >= requiredUnits);
        Assert.True(requiredUnits - lotUnitsExcludingLast > 0);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(150)]
    [InlineData(200)]
    [InlineData(225)]
    public void GetGoodLotsFifoForStockDemand_ReturnsOldestLotsWithEnoughUnits_WhenUnitsStranded(uint requiredUnits)
    {
        var inventory = GetTestInventory();
        inventory.TurnStranded();

        var lots = inventory.GetGoodLotsFifoForStockDemand(requiredUnits);

        var remainingLots = inventory.StrandedLots.Except(lots);
        var lotUnits = (uint)lots.Sum(x => x.UnitsInStock);
        var lastLot = lots.TakeLast(1).Single();
        var lotUnitsExcludingLast = lotUnits - lastLot.UnitsInStock;
        Assert.True(lots.All(x => x.IsUnitsStranded));
        Assert.True(remainingLots.All(x => lots.All(
            l => l.DateUnitsBecameStranded <= x.DateUnitsBecameStranded)));
        Assert.True(lotUnits >= requiredUnits);
        Assert.True(requiredUnits - lotUnitsExcludingLast > 0);
    }

    [Fact]
    public void GetGoodLotsFifoForStockDemand_ThrowsArgOutOfRangeException_WhenUnitsIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var lots = inventory.GetGoodLotsFifoForStockDemand(0u);
        });
    }

    [Fact]
    public void GetGoodLotsFifoForStockDemand_ThrowsNotEnoughUnitsException_WhenNotEnoughUnits()
    {
        var inventory = GetTestInventory();

        Assert.Throws<NotEnoughUnitsException>(() =>
        {
            var lots = inventory.GetGoodLotsFifoForStockDemand(inventory.FulfillableUnits + 1);
        });
    }

    [Theory]
    [InlineData(12)]
    [InlineData(15)]
    [InlineData(22)]
    [InlineData(25)]
    [InlineData(28)]
    [InlineData(30)]
    public void GetUnfulfillabbleLotsFifoForStockDemand_ReturnsOldestLotsWithEnoughUnits(uint requiredUnits)
    {
        var inventory = GetTestInventory();

        var lots = inventory.GetUnfulfillabbleLotsFifoForStockDemand(requiredUnits);

        var remainingLots = inventory.UnfulfillableLots.Except(lots);
        var lotUnits = (uint)lots.Sum(x => x.UnitsInStock);
        var lastLot = lots.TakeLast(1).Single();
        var lotUnitsExcludingLast = lotUnits - lastLot.UnitsInStock;
        Assert.True(lots.All(x => x.IsUnitsUnfulfillable));
        Assert.True(remainingLots.All(x => lots.All(
            l => l.DateUnitsBecameUnfulfillable <= x.DateUnitsBecameUnfulfillable)));
        Assert.True(lotUnits >= requiredUnits);
        Assert.True(requiredUnits - lotUnitsExcludingLast > 0);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void ReceiveGoodStock_AddsGoodStockToExistingLotOrCreateNewLotIfNotExists_WhenAllValid(
        bool lotExists, bool isUnitsStranded)
    {
        var inventory = lotExists ? GetTestInventory()
            : GetTestInventoryWithoutLotForCurrentDate();
        uint units = 100;
        var expectedLotCount = inventory.FulfillableLots.Count + (lotExists ? 0 : 1);
        var expectedUnits = inventory.FulfillableUnits + units;
        if (isUnitsStranded)
        {
            inventory.TurnStranded();
            expectedLotCount = inventory.StrandedLots.Count + (lotExists ? 0 : 1);
            expectedUnits = inventory.StrandedUnits + units;
        }

        inventory.ReceiveGoodStock(units);

        var actualUnits = isUnitsStranded
            ? inventory.StrandedUnits : inventory.FulfillableUnits;
        var actualLotCount = isUnitsStranded
            ? inventory.StrandedLots.Count : inventory.FulfillableLots.Count;
        Assert.Equal(expectedUnits, actualUnits);
        Assert.Equal(expectedLotCount, actualLotCount);
    }

    [Fact]
    public void ReceiveGoodStock_ThrowsArgOutOfRangeException_WhenUnitsToReceiveIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.ReceiveGoodStock(0u);
        });
    }

    [Fact]
    public void ReceiveGoodStock_ThrowsExceedingMaxStockThresholdException_WhenNewStockExceedsMaxStockThreshold()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            inventory.ReceiveGoodStock(inventory.MaxStockThreshold - inventory.TotalUnits + 1);
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

        inventory.AddUnfulfillableStock(units, DateTime.Now.Date,
            DateTime.Now.Date, UnfulfillableCategory.Defective);

        Assert.Equal(unfulfillableUnitsInStockAfterAddition, inventory.UnfulfillableUnits);
        Assert.Equal(lotCountAfterAddition, inventory.UnfulfillableLots.Count);
    }

    [Fact]
    public void AddUnfulfillableStock_ThrowsArgOutOfRangeException_WhenUnitsToReduceIsZero()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            inventory.AddUnfulfillableStock(0u, DateTime.Now.Date,
                DateTime.Now.Date, UnfulfillableCategory.Defective);
        });
    }

    [Fact]
    public void AddUnfulfillableStock_ThrowsExceedingMaxStockThresholdException_WhenNewStockExceedsMaxStockThreshold()
    {
        var inventory = GetTestInventory();

        Assert.Throws<ExceedingMaxStockThresholdException>(() =>
        {
            inventory.AddUnfulfillableStock(inventory.MaxStockThreshold - inventory.TotalUnits + 1,
                DateTime.Now.Date, DateTime.Now.Date, UnfulfillableCategory.Defective);
        });
    }

    [Fact]
    public void TurnStranded_TurnsAllLotsStrandedAndSetsIsStrandedTrue()
    {
        var inventory = GetTestInventory();
        var initialFulfillableLots = inventory.FulfillableLots;
        var initialFulfillableUnits = inventory.FulfillableUnits;

        inventory.TurnStranded();

        Assert.True(inventory.IsStranded);
        Assert.Equal(initialFulfillableUnits, inventory.StrandedUnits);
        Assert.Equal(0u, inventory.FulfillableUnits);
        Assert.DoesNotContain(inventory.StrandedLots, x => !initialFulfillableLots.Contains(x));
    }

    [Fact]
    public void ConfirmStrandingResolved_TurnsAllLotsBackToFulfillableAndSetsIsStrandedFalse()
    {
        var inventory = GetTestInventory();
        inventory.TurnStranded();
        var initialStrandedLots = inventory.StrandedLots;
        var initialStrandedUnits = inventory.StrandedUnits;

        inventory.ConfirmStrandingResolved();

        Assert.True(!inventory.IsStranded);
        Assert.Equal(initialStrandedUnits, inventory.FulfillableUnits);
        Assert.Equal(0u, inventory.StrandedUnits);
        Assert.DoesNotContain(inventory.FulfillableLots, x => !initialStrandedLots.Contains(x));
    }

    [Fact]
    public void RemoveEmptyLots_AlwaysSucceeds()
    {
        var inventory = GetTestInventory();
        var lotsToRemove = new List<Lot>
        {
            inventory.Lots.First(x => x.IsUnitsFulfillable),
            inventory.Lots.First(x => x.IsUnitsUnfulfillable),
        };
        foreach (var lot in lotsToRemove)
        {
            if (lot.UnitsInStock > 0)
                lot.AdjustUnits(-(int)lot.UnitsInStock);
            if (lot.UnitsInRemoval > 0)
                lot.ConfirmUnitsRemoved(lot.UnitsInRemoval);
        }

        inventory.RemoveEmptyLots();

        Assert.DoesNotContain(inventory.Lots, lot => lot.TotalUnits == 0);
        Assert.DoesNotContain(inventory.Lots, lotsToRemove.Contains);
    }
}
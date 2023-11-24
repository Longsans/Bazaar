namespace Bazaar.FbbInventory.Domain.Entities;

public class ProductInventory
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }

    private readonly List<FulfillableLot> _fulfillableLots;
    public IReadOnlyCollection<FulfillableLot> FulfillableLots
        => _fulfillableLots.AsReadOnly();
    private readonly List<UnfulfillableLot> _unfulfillableLots;
    public IReadOnlyCollection<UnfulfillableLot> UnfulfillableLots
        => _unfulfillableLots.AsReadOnly();

    public uint FulfillableUnitsInStock
        => (uint)_fulfillableLots.Sum(x => x.UnitsInStock);
    public uint FulfillableUnitsPendingRemoval
        => (uint)_fulfillableLots.Sum(x => x.UnitsPendingRemoval);
    public uint UnfulfillableUnitsInStock
        => (uint)_unfulfillableLots.Sum(x => x.UnitsInStock);
    public uint UnfulfillableUnitsPendingRemoval
        => (uint)_unfulfillableLots.Sum(x => x.UnitsPendingRemoval);

    public uint TotalUnits => FulfillableUnitsInStock + FulfillableUnitsPendingRemoval
            + UnfulfillableUnitsInStock + UnfulfillableUnitsPendingRemoval;

    public uint RestockThreshold { get; private set; }
    public uint MaxStockThreshold { get; private set; }
    public SellerInventory SellerInventory { get; private set; }
    public int SellerInventoryId { get; private set; }

    public bool HasPickupsInProgress { get; private set; }

    public ProductInventory(
        string productId, uint fulfillableUnits,
        uint defectiveUnits, uint warehouseDamagedUnits,
        uint restockThreshold, uint maxStockThreshold, int sellerInventoryId)
    {
        if (restockThreshold > maxStockThreshold ||
            fulfillableUnits + defectiveUnits + warehouseDamagedUnits > maxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        ProductId = productId;
        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
        SellerInventoryId = sellerInventoryId;

        _fulfillableLots = new()
        {
            new(this, fulfillableUnits)
        };

        _unfulfillableLots = new();
        if (defectiveUnits != 0)
        {
            _unfulfillableLots.Add(new(this,
                defectiveUnits, UnfulfillableCategory.Defective));
        }
        if (warehouseDamagedUnits != 0)
        {
            _unfulfillableLots.Add(new(this,
                warehouseDamagedUnits, UnfulfillableCategory.WarehouseDamaged));
        }
    }

    // EF read constructor
    private ProductInventory() { }

    #region Domain logic
    public void ReduceFulfillableStock(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to reduce cannot be 0.");
        }
        if (units > FulfillableUnitsInStock)
        {
            throw new NotEnoughStockException("Not enough fulfillable stock.");
        }

        var fulfillableLotsFromOldest = _fulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.DateEnteredStorage)
            .ToList();
        ReduceUnitsInStockWithAltIteration(fulfillableLotsFromOldest,
            _fulfillableLots, units);
    }

    public void ReduceUnfulfillableStock(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to reduce cannot be 0.");
        }
        if (units > UnfulfillableUnitsInStock)
        {
            throw new NotEnoughStockException("Not enough unfulfillable stock.");
        }

        var unfulfillableLotsFromOldest = _unfulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.DateUnfulfillableSince)
            .ToList();
        ReduceUnitsInStockWithAltIteration(unfulfillableLotsFromOldest,
            _unfulfillableLots, units);
    }

    public void LabelFulfillableUnitsForRemoval(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to reduce cannot be 0.");
        }
        if (units > FulfillableUnitsInStock)
        {
            throw new NotEnoughStockException("Not enough fulfillable stock.");
        }

        var fulfillableLotsFromOldest = _fulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.DateEnteredStorage);
        LabelUnitsForRemovalFromOldest(fulfillableLotsFromOldest, units);
    }

    public void LabelUnfulfillableUnitsForRemoval(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to reduce cannot be 0.");
        }
        if (units > UnfulfillableUnitsInStock)
        {
            throw new NotEnoughStockException("Not enough unfulfillable stock.");
        }

        var unfulfillableLotsFromOldest = _unfulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.DateUnfulfillableSince);
        LabelUnitsForRemovalFromOldest(unfulfillableLotsFromOldest, units);
    }

    public void AddFulfillableStock(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to restock cannot be 0.");
        }
        if (TotalUnits + units > MaxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        var fulfillableQty = _fulfillableLots
            .SingleOrDefault(x => x.DateEnteredStorage == DateTime.Now.Date);
        if (fulfillableQty == null)
        {
            _fulfillableLots.Add(new(this, units));
        }
        else
        {
            fulfillableQty.AddStock(units);
        }
    }

    public void AddUnfulfillableStock(UnfulfillableCategory category, uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to add cannot be 0.");
        }
        if (TotalUnits + units > MaxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        var unfulfillableLot = _unfulfillableLots
            .Where(x => x.UnfulfillableCategory == category
                && x.DateUnfulfillableSince == DateTime.Now.Date)
            .SingleOrDefault();

        if (unfulfillableLot == null)
        {
            _unfulfillableLots.Add(
                new UnfulfillableLot(this, units, category));
        }
        else
        {
            unfulfillableLot.AddStock(units);
        }
    }

    public void RemoveEmptyLots()
    {
        _fulfillableLots.RemoveAll(x => x.TotalUnits == 0);
        _unfulfillableLots.RemoveAll(x => x.TotalUnits == 0);
    }

    public void UpdateHasPickupsInProgress(bool hasPickupsInProgress)
    {
        HasPickupsInProgress = hasPickupsInProgress;
    }
    #endregion

    #region Helpers
    private static void ReduceUnitsInStockWithAltIteration<T>(IEnumerable<T> iterateList,
        IList<T> originalList, uint totalReduceUnits)
        where T : Lot
    {
        foreach (var lot in iterateList)
        {
            if (totalReduceUnits >= lot.UnitsInStock)
            {
                originalList.Remove(lot);
                totalReduceUnits -= lot.UnitsInStock;
            }
            else
            {
                lot.ReduceStock(totalReduceUnits);
                totalReduceUnits = 0;
            }
            if (totalReduceUnits == 0)
            {
                break;
            }
        }
    }

    private static void LabelUnitsForRemovalFromOldest(
        IEnumerable<Lot> lots, uint totalUnitsToMark)
    {
        foreach (var lot in lots)
        {
            var unitsToRemove = totalUnitsToMark > lot.UnitsInStock
                ? lot.UnitsInStock
                : totalUnitsToMark;
            lot.LabelUnitsInStockForRemoval(unitsToRemove);
            totalUnitsToMark -= unitsToRemove;

            if (totalUnitsToMark == 0)
                break;
        }
    }
    #endregion
}
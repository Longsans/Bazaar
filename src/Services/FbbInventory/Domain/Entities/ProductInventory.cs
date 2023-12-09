namespace Bazaar.FbbInventory.Domain.Entities;

public class ProductInventory
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }

    private readonly List<Lot> _lots;
    public IReadOnlyCollection<Lot> Lots => _lots.AsReadOnly();
    public IReadOnlyCollection<Lot> FulfillableLots
        => _lots.Where(x => !x.IsUnfulfillable).ToList().AsReadOnly();
    public IReadOnlyCollection<Lot> UnfulfillableLots
        => _lots.Where(x => x.IsUnfulfillable).ToList().AsReadOnly();

    public uint FulfillableUnitsInStock
        => (uint)FulfillableLots.Sum(x => x.UnitsInStock);
    public uint FulfillableUnitsPendingRemoval
        => (uint)FulfillableLots.Sum(x => x.UnitsPendingRemoval);
    public uint UnfulfillableUnitsInStock
        => (uint)UnfulfillableLots.Sum(x => x.UnitsInStock);
    public uint UnfulfillableUnitsPendingRemoval
        => (uint)UnfulfillableLots.Sum(x => x.UnitsPendingRemoval);

    public uint TotalUnits => FulfillableUnitsInStock + FulfillableUnitsPendingRemoval
            + UnfulfillableUnitsInStock + UnfulfillableUnitsPendingRemoval;

    public uint RemainingCapacity => MaxStockThreshold - TotalUnits;

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
        var totalUnits = fulfillableUnits + defectiveUnits + warehouseDamagedUnits;
        if (totalUnits == 0)
        {
            throw new ArgumentOutOfRangeException(
                "The total number of units in product inventory cannot be 0.", null as Exception);
        }
        if (restockThreshold > maxStockThreshold
            || fulfillableUnits + defectiveUnits + warehouseDamagedUnits > maxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        ProductId = productId;
        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
        SellerInventoryId = sellerInventoryId;

        _lots = new();
        if (fulfillableUnits != 0)
        {
            _lots.Add(new(this, fulfillableUnits));
        }
        if (defectiveUnits != 0)
        {
            _lots.Add(new(this, UnfulfillableCategory.Defective, defectiveUnits));
        }
        if (warehouseDamagedUnits != 0)
        {
            _lots.Add(new(this, UnfulfillableCategory.WarehouseDamaged, warehouseDamagedUnits));
        }
    }

    // EF read constructor
    private ProductInventory() { }

    #region Domain logic
    public void ReduceFulfillableStockFromOldToNew(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to reduce cannot be 0.");
        }
        if (units > FulfillableUnitsInStock)
        {
            throw new NotEnoughUnitsException("Not enough fulfillable stock.");
        }

        var fulfillableLotsFromOldest = FulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.TimeEnteredStorage)
            .ToList();
        ReduceUnitsInStockWithAltIteration(fulfillableLotsFromOldest,
            _lots, units);
    }

    public void ReduceUnfulfillableStockFromOldToNew(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to reduce cannot be 0.");
        }
        if (units > UnfulfillableUnitsInStock)
        {
            throw new NotEnoughUnitsException("Not enough unfulfillable stock.");
        }

        var unfulfillableLotsFromOldest = UnfulfillableLots
            .Where(x => x.HasUnitsInStock)
            .OrderBy(x => x.TimeEnteredStorage)
            .ToList();
        ReduceUnitsInStockWithAltIteration(unfulfillableLotsFromOldest,
            _lots, units);
    }

    public void AddFulfillableStock(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to restock cannot be 0.");
        }
        if (units > RemainingCapacity)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        var fulfillableQty = FulfillableLots
            .SingleOrDefault(x => x.TimeEnteredStorage == DateTime.Now.Date);
        if (fulfillableQty == null)
        {
            _lots.Add(new(this, units));
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
        if (units > RemainingCapacity)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        var unfulfillableLot = UnfulfillableLots
            .Where(x => x.UnfulfillableCategory == category
                && x.TimeEnteredStorage == DateTime.Now.Date)
            .SingleOrDefault();

        if (unfulfillableLot == null)
        {
            _lots.Add(new(this, category, units));
        }
        else
        {
            unfulfillableLot.AddStock(units);
        }
    }

    public void LabelAllFulfillableStockAsStrandedStock()
    {
        foreach (var lot in FulfillableLots)
        {
            lot.LabelUnfulfillable(UnfulfillableCategory.Stranded);
        }
    }

    public void RelabelStrandedStockAsFulfillableStock()
    {
        foreach (var lot in UnfulfillableLots
            .Where(x => x.UnfulfillableCategory == UnfulfillableCategory.Stranded))
        {
            lot.RemoveUnfulfillableLabel();
        }
    }

    public void RemoveEmptyLots()
    {
        _lots.RemoveAll(x => x.TotalUnits == 0);
    }

    public void UpdateHasPickupsInProgress(bool hasPickupsInProgress)
    {
        HasPickupsInProgress = hasPickupsInProgress;
    }
    #endregion

    #region Helpers
    private static void ReduceUnitsInStockWithAltIteration<T>(IList<T> iterateList,
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
    #endregion
}
namespace Bazaar.FbbInventory.Domain.Entities;

public class Lot
{
    public int Id { get; protected set; }
    public string LotNumber { get; protected set; }
    public uint UnitsInStock { get; protected set; }
    public uint UnitsPendingRemoval { get; protected set; }
    public uint TotalUnits { get; protected set; }
    public DateTime TimeEnteredStorage { get; protected set; }
    public DateTime? TimeUnfulfillableSince { get; protected set; }
    public UnfulfillableCategory? UnfulfillableCategory { get; protected set; }
    public ProductInventory ProductInventory { get; protected set; }
    public int ProductInventoryId { get; protected set; }

    public bool IsUnfulfillable => UnfulfillableCategory != null;
    public bool HasUnitsInStock => UnitsInStock > 0;
    public bool IsUnfulfillableBeyondPolicyDuration
        => TimeUnfulfillableSince + StoragePolicy.MaximumUnfulfillableDuration <= DateTime.Now.Date;

    // Fulfillable lot
    public Lot(
        ProductInventory inventory, uint unitsInStock)
    {
        if (unitsInStock == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitsInStock),
                "Fulfillable units cannot be 0.");
        }

        TimeEnteredStorage = DateTime.Now;
        UnitsInStock = unitsInStock;
        ProductInventory = inventory;
        ProductInventoryId = inventory.Id;
        UpdateTotalUnits();
    }

    // Unfulfillable lot
    public Lot(
        ProductInventory inventory, UnfulfillableCategory unfulfillableCategory, uint unitsInStock)
    {
        if (unitsInStock == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitsInStock),
                "Fulfillable units cannot be 0.");
        }

        TimeEnteredStorage = DateTime.Now;
        TimeUnfulfillableSince = DateTime.Now;
        UnfulfillableCategory = unfulfillableCategory;
        UnitsInStock = unitsInStock;
        ProductInventory = inventory;
        ProductInventoryId = inventory.Id;
        UpdateTotalUnits();
    }

    protected Lot() { }

    public void ReduceStock(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(units),
                "Units cannot be 0.");
        }
        if (units > UnitsInStock)
        {
            throw new NotEnoughUnitsException();
        }

        UnitsInStock -= units;
        UpdateTotalUnits();
    }

    public void AddStock(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(units),
                "Units cannot be 0.");
        }
        if (ProductInventory.TotalUnits + units > ProductInventory.MaxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        UnitsInStock += units;
        UpdateTotalUnits();
    }

    public void LabelUnitsInStockForRemoval(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(units),
                "Units cannot be 0.");
        }
        if (units > UnitsInStock)
        {
            throw new NotEnoughUnitsException(
                "Number of units pending removal exceeds total number of units.");
        }

        UnitsInStock -= units;
        UnitsPendingRemoval += units;
        UpdateTotalUnits();
    }

    public void RemovePendingUnits(uint unitsToRemove)
    {
        if (unitsToRemove == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitsToRemove),
                "Units cannot be 0.");
        }
        if (unitsToRemove > UnitsPendingRemoval)
        {
            throw new NotEnoughUnitsException(
                "Units pending removal are fewer than the units requested to remove.");
        }

        UnitsPendingRemoval -= unitsToRemove;
        UpdateTotalUnits();
    }

    public void ReturnPendingUnitsToStock(uint unitsToReturn)
    {
        if (unitsToReturn == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitsToReturn),
                "Units cannot be 0.");
        }
        if (unitsToReturn > UnitsPendingRemoval)
        {
            throw new NotEnoughUnitsException(
                "Units pending removal are fewer than the units requested to return.");
        }

        UnitsPendingRemoval -= unitsToReturn;
        UnitsInStock += unitsToReturn;
        UpdateTotalUnits();
    }

    public void LabelUnfulfillable(UnfulfillableCategory category)
    {
        UnfulfillableCategory = category;
        TimeUnfulfillableSince = DateTime.Now;
    }

    public void RemoveUnfulfillableLabel()
    {
        if (UnfulfillableCategory != null
            && UnfulfillableCategory != Entities.UnfulfillableCategory.Stranded)
        {
            throw new InvalidOperationException(
                "Cannot remove unfulfillable label on damaged goods.");
        }
        UnfulfillableCategory = null;
        TimeUnfulfillableSince = null;
    }

    protected void UpdateTotalUnits()
    {
        TotalUnits = UnitsInStock + UnitsPendingRemoval;
    }
}

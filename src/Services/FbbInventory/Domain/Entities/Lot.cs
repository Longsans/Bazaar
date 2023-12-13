namespace Bazaar.FbbInventory.Domain.Entities;

public class Lot
{
    public int Id { get; private set; }
    public string LotNumber { get; private set; }
    public uint UnitsInStock { get; private set; }
    public uint UnitsPendingRemoval { get; private set; }
    public uint TotalUnits { get; private set; }
    public DateTime TimeEnteredStorage { get; private set; }
    public DateTime? TimeUnfulfillableSince { get; private set; }
    public UnfulfillableCategory? UnfulfillableCategory { get; private set; }
    public ProductInventory ProductInventory { get; private set; }
    public int ProductInventoryId { get; private set; }

    public bool IsUnfulfillable => UnfulfillableCategory != null && TimeUnfulfillableSince != null;
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
        if (!Enum.IsDefined(typeof(UnfulfillableCategory), unfulfillableCategory))
        {
            throw new ArgumentOutOfRangeException(nameof(unfulfillableCategory),
                "Unfulfillable category does not exist.");
        }
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
        TimeUnfulfillableSince = DateTime.Now;
        UnfulfillableCategory = category;
    }

    public void RemoveUnfulfillableLabel()
    {
        if (UnfulfillableCategory != null
            && UnfulfillableCategory != Entities.UnfulfillableCategory.Stranded)
        {
            throw new InvalidOperationException(
                "Cannot remove unfulfillable label on damaged goods.");
        }
        TimeUnfulfillableSince = null;
        UnfulfillableCategory = null;
    }

    protected void UpdateTotalUnits()
    {
        TotalUnits = UnitsInStock + UnitsPendingRemoval;
    }
}

public enum UnfulfillableCategory
{
    Defective,
    WarehouseDamaged,
    CustomerDamaged,
    Stranded
}
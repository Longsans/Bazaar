namespace Bazaar.FbbInventory.Domain.Entities;

public class Lot
{
    public int Id { get; protected set; }
    public string LotNumber { get; protected set; }
    public uint UnitsInStock { get; protected set; }
    public uint UnitsPendingRemoval { get; protected set; }
    public uint TotalUnits { get; protected set; }
    public ProductInventory ProductInventory { get; protected set; }
    public int ProductInventoryId { get; protected set; }

    public bool HasUnitsInStock => UnitsInStock > 0;

    public Lot(
        ProductInventory inventory, uint unitsInStock)
    {
        if (unitsInStock == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitsInStock),
                "Fulfillable units cannot be 0.");
        }

        ProductInventory = inventory;
        ProductInventoryId = inventory.Id;
        UnitsInStock = unitsInStock;
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
            throw new NotEnoughStockException();
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
            throw new NotEnoughStockException(
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
            throw new ArgumentOutOfRangeException(nameof(unitsToRemove),
                "Units pending removal are fewer than the number requested to remove.");
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
            throw new ArgumentOutOfRangeException(nameof(unitsToReturn),
                "Units pending removal are fewer than the number requested to return.");
        }

        UnitsPendingRemoval -= unitsToReturn;
        UnitsInStock += unitsToReturn;
        UpdateTotalUnits();
    }

    protected void UpdateTotalUnits()
    {
        TotalUnits = UnitsInStock + UnitsPendingRemoval;
    }
}

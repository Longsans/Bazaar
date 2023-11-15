namespace Bazaar.FbbInventory.Domain.Entities;

public class ProductInventory
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public uint UnitsInStock { get; private set; }
    public uint RestockThreshold { get; private set; }
    public uint MaxStockThreshold { get; private set; }
    public SellerInventory SellerInventory { get; private set; }
    public int SellerInventoryId { get; private set; }

    public InventoryStatus Status { get; private set; }

    public bool HasPickupsInProgress { get; private set; }
    public DateTime? UnfulfillableSince { get; private set; }

    public ProductInventory(
        string productId, uint unitsInStock,
        uint restockThreshold, uint maxStockThreshold, int sellerInventoryId)
    {
        if (restockThreshold > maxStockThreshold || unitsInStock > maxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        ProductId = productId;
        UnitsInStock = unitsInStock;
        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
        SellerInventoryId = sellerInventoryId;

        if (unitsInStock > 0)
        {
            Status = InventoryStatus.Ready;
        }
        else
        {
            Status = InventoryStatus.Unfulfillable;
            UnfulfillableSince = DateTime.Now.Date;
        }
    }

    // EF read constructor
    private ProductInventory() { }

    public void ReduceStock(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to reduce must be larger than 0.");
        }
        if (units > UnitsInStock)
        {
            throw new NotEnoughStockException();
        }

        UnitsInStock -= units;
        if (UnitsInStock == 0)
        {
            MarkAsUnfulfillable();
        }
    }

    public void Restock(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to restock must be larger than 0.");
        }
        if (UnitsInStock + units > MaxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        UnitsInStock += units;
        if (Status == InventoryStatus.Unfulfillable)
        {
            Status = InventoryStatus.Ready;
        }
    }

    public void MarkAsUnfulfillable()
    {
        if (Status != InventoryStatus.Ready)
        {
            throw new InvalidOperationException(
                "Can only mark inventory unfulfillable if it's currently in Ready status.");
        }

        Status = InventoryStatus.Unfulfillable;
        UnfulfillableSince = DateTime.Now.Date;
    }

    public void MarkToBeDisposed()
    {
        if (Status == InventoryStatus.Ready)
        {
            throw new InvalidOperationException(
                "Can only mark inventory to be disposed if it's currently in Unfulfillable status.");
        }

        Status = InventoryStatus.ToBeDisposed;
    }

    public void UpdateHasPickupsInProgress(bool hasPickupsInProgress)
    {
        HasPickupsInProgress = hasPickupsInProgress;
    }
}

public enum InventoryStatus
{
    Unfulfillable,
    Ready,
    ToBeDisposed
}
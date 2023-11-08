namespace Bazaar.Inventory.Domain.Entities;

public class ProductInventory
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public uint UnitsInStock { get; private set; }
    public uint RestockThreshold { get; private set; }
    public uint MaxStockThreshold { get; private set; }
    public SellerInventory SellerInventory { get; private set; }
    public int SellerInventoryId { get; private set; }

    public ProductInventory(
        string productId, uint unitsInStock,
        uint restockThreshold, uint maxStockThreshold, int sellerInventoryId)
    {
        if (RestockThreshold > maxStockThreshold || unitsInStock > maxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        ProductId = productId;
        UnitsInStock = unitsInStock;
        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
        SellerInventoryId = sellerInventoryId;
    }

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
    }
}

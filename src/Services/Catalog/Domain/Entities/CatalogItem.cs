namespace Bazaar.Catalog.Domain.Entities;

public class CatalogItem
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string ProductDescription { get; private set; }
    public decimal Price { get; private set; }
    public uint AvailableStock { get; private set; }
    public string SellerId { get; private set; }

    // Available stock at which we should reorder
    public uint RestockThreshold { get; private set; }

    // Maximum number of units that can be in-stock at any time
    // (due to physicial/logistical constraints in warehouses)
    public uint MaxStockThreshold { get; private set; }

    // The following 3 properties are used in service integration
    public bool IsFulfilledByBazzar { get; private set; }
    public bool IsOfficiallyListed { get; private set; }
    public bool HasNoOrdersInProgress { get; private set; }

    public bool IsDeleted { get; private set; }

    public CatalogItem(
        int id, string productId, string productName, string productDescription,
        decimal price, uint availableStock, string sellerId,
        uint restockThreshold, uint maxStockThreshold)
    {
        if (price <= 0m)
            throw new ArgumentException("Product price cannot be 0 or negative.");

        if (availableStock == 0)
            throw new ArgumentException("Available stock cannot be 0.");

        if (maxStockThreshold == 0)
            throw new ArgumentException("Max stock threshold cannot be 0.");

        if (restockThreshold >= maxStockThreshold)
            throw new ArgumentException("Restock threshold must be less than max stock threshold.");

        if (maxStockThreshold < availableStock)
            throw new ExceedingMaxStockThresholdException();

        Id = id;
        ProductId = productId;
        ProductName = productName;
        ProductDescription = productDescription;
        Price = price;
        AvailableStock = availableStock;
        SellerId = sellerId;
        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
    }

    public void ChangeProductDetails(string productName, string productDescription, decimal price)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Item deleted.");

        if (price <= 0m)
            throw new ArgumentException("Product price cannot be 0 or negative.");

        ProductName = productName;
        ProductDescription = productDescription;
        Price = price;
    }

    public void ReduceStock(uint units)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Item deleted.");

        if (units == 0)
            throw new ArgumentException("Number of units to reduce must be greater than 0.");

        if (units > AvailableStock)
            throw new NotEnoughStockException();

        AvailableStock -= units;
    }

    public void Restock(uint units)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Item deleted.");

        if (units == 0)
            throw new ArgumentException("Number of units to restock must be greater than 0.");

        if (AvailableStock + units > MaxStockThreshold)
            throw new ExceedingMaxStockThresholdException();

        AvailableStock += units;
    }

    public void ChangeStockThresholds(uint restockThreshold, uint maxStockThreshold)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Item deleted.");

        if (maxStockThreshold == 0)
            throw new ArgumentException("Max stock threshold cannot be 0.");

        if (restockThreshold >= maxStockThreshold)
            throw new ArgumentException("Restock threshold must be less than max stock threshold.");

        if (maxStockThreshold < AvailableStock)
            throw new ExceedingMaxStockThresholdException();

        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
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

    // The following 3 properties are used in service integration
    public bool IsFulfilledByBazaar { get; private set; }
    public bool IsOfficiallyListed { get; private set; }
    public bool HasOrdersInProgress { get; private set; }

    public bool IsDeleted { get; private set; }

    public CatalogItem(
        int id, string productId, string productName, string productDescription,
        decimal price, uint availableStock, string sellerId, bool isFulfilledByBazaar)
    {
        if (price <= 0m)
            throw new ArgumentException("Product price cannot be 0 or negative.");

        Id = id;
        ProductId = productId;
        ProductName = productName;
        ProductDescription = productDescription;
        Price = price;
        AvailableStock = availableStock;
        SellerId = sellerId;
        IsFulfilledByBazaar = isFulfilledByBazaar;
        IsOfficiallyListed = !isFulfilledByBazaar;
    }

    [Newtonsoft.Json.JsonConstructor]
    private CatalogItem(
        int id, string productId, string productName, string productDescription,
        decimal price, uint availableStock, string sellerId, bool isFulfilledByBazaar, bool hasOrdersInProgress)
        : this(id, productId, productName, productDescription, price, availableStock, sellerId, isFulfilledByBazaar)
    {
        HasOrdersInProgress = hasOrdersInProgress;
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

        AvailableStock += units;
    }

    public void UpdateHasOrdersInProgressStatus(bool hasOrdersInProgress)
    {
        HasOrdersInProgress = hasOrdersInProgress;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
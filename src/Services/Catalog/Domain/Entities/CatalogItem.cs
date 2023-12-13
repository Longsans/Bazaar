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
    public ListingStatus ListingStatus { get; private set; }
    public FulfillmentMethod FulfillmentMethod { get; private set; }

    public bool HasOrdersInProgress { get; private set; }

    public bool IsFbb => FulfillmentMethod == FulfillmentMethod.Fbb;
    public bool IsListingActive => ListingStatus == ListingStatus.Active;
    public bool IsOutOfStock => ListingStatus == ListingStatus.InactiveOutOfStock;
    public bool IsListingClosed => ListingStatus == ListingStatus.InactiveClosedListing;
    public bool IsDeleted => ListingStatus == ListingStatus.Deleted;

    // Create constructor
    public CatalogItem(
        string productName, string productDescription,
        decimal price, uint availableStock, string sellerId, FulfillmentMethod fulfillmentMethod)
    {
        if (price <= 0m)
            throw new ArgumentOutOfRangeException(
                nameof(price), "Product price cannot be 0 or negative.");

        if (availableStock > 0 && fulfillmentMethod == FulfillmentMethod.Fbb)
            throw new ManualInsertOfFbbStockNotSupportedException();

        ProductName = productName;
        ProductDescription = productDescription;
        Price = price;
        AvailableStock = availableStock;
        SellerId = sellerId;
        FulfillmentMethod = fulfillmentMethod;
        ListingStatus = availableStock > 0
            ? ListingStatus.Active : ListingStatus.InactiveOutOfStock;
    }

    [Newtonsoft.Json.JsonConstructor]
    private CatalogItem(
        string productName,
        string productDescription, decimal price, uint availableStock,
        string sellerId, FulfillmentMethod fulfillmentMethod, bool hasOrdersInProgress)
    {
        ProductName = productName;
        ProductDescription = productDescription;
        Price = price;
        AvailableStock = availableStock;
        SellerId = sellerId;
        FulfillmentMethod = fulfillmentMethod;
        ListingStatus = availableStock > 0
            ? ListingStatus.Active : ListingStatus.InactiveOutOfStock;
        HasOrdersInProgress = hasOrdersInProgress;
    }

    // EF read constructor
    private CatalogItem() { }

    public void ChangeProductDetails(string productName, string productDescription, decimal price)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Item deleted.");

        if (price <= 0m)
            throw new ArgumentOutOfRangeException(
                nameof(price), "Product price cannot be 0 or negative.");

        ProductName = productName;
        ProductDescription = productDescription;
        Price = price;
    }

    public void ReduceStock(uint units)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Item deleted.");

        if (units == 0)
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of units to reduce must be greater than 0.");

        if (units > AvailableStock)
            throw new NotEnoughStockException();

        AvailableStock -= units;
        if (AvailableStock == 0 && IsListingActive)
            ListingStatus = ListingStatus.InactiveOutOfStock;
    }

    public void Restock(uint units)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Item deleted.");

        if (units == 0)
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of units to restock must be greater than 0.");

        AvailableStock += units;
        if (IsOutOfStock)
            ListingStatus = ListingStatus.Active;
    }

    public void CloseListing()
    {
        ListingStatus = !IsDeleted
            ? ListingStatus.InactiveClosedListing
            : throw new InvalidOperationException();
    }

    public void Relist()
    {
        ListingStatus = IsListingClosed
            ? ListingStatus.Active
            : throw new InvalidOperationException();
    }

    public void Delete()
    {
        ListingStatus = ListingStatus.Deleted;
    }

    public void ChangeFulfillmentMethodToFbb()
    {
        if (IsDeleted || IsListingClosed)
        {
            throw new InvalidOperationException(
                "Cannot change fulfillment method of deleted or closed listing.");
        }
        if (IsFbb)
        {
            throw new InvalidOperationException();
        }

        FulfillmentMethod = FulfillmentMethod.Fbb;
        if (AvailableStock > 0)
            ReduceStock(AvailableStock);
    }

    public void ChangeFulfillmentMethodToMerchant()
    {
        if (IsDeleted || IsListingClosed)
        {
            throw new InvalidOperationException(
                "Cannot change fulfillment method of deleted or closed listing.");
        }

        FulfillmentMethod = FulfillmentMethod.Merchant;
    }

    public void UpdateHasOrdersInProgressStatus(bool hasOrdersInProgress)
    {
        HasOrdersInProgress = hasOrdersInProgress;
    }
}
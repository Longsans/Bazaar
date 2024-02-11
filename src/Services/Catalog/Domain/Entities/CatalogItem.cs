namespace Bazaar.Catalog.Domain.Entities;

public class CatalogItem
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string ProductDescription { get; private set; }
    public decimal Price { get; private set; }
    public string? ImageFilename { get; private set; }
    public decimal ProductLengthCm { get; private set; }
    public decimal ProductWidthCm { get; private set; }
    public decimal ProductHeightCm { get; private set; }
    public uint AvailableStock { get; private set; }
    public ProductCategory MainDepartment { get; private set; }
    public ProductCategory Subcategory { get; private set; }
    public int MainDepartmentId { get; private set; }
    public int SubcategoryId { get; private set; }

    public string SellerId { get; private set; }
    public ListingStatus ListingStatus { get; private set; }
    public FulfillmentMethod FulfillmentMethod { get; private set; }

    public bool HasOrdersInProgress { get; private set; }

    public bool IsFbb => FulfillmentMethod == FulfillmentMethod.Fbb;
    public bool IsListingActive => ListingStatus == ListingStatus.Active;
    public bool IsOutOfStock => ListingStatus == ListingStatus.InactiveOutOfStock;
    public bool IsListingClosed => ListingStatus == ListingStatus.InactiveClosedListing;
    public bool IsDeleted => ListingStatus == ListingStatus.Deleted;
    public bool IsDeletable => !HasOrdersInProgress && (!IsFbb || AvailableStock == 0);

    // Create constructor
    public CatalogItem(
        string productName, string productDescription,
        decimal price, uint availableStock, ProductCategory subcategory,
        decimal productLengthCm, decimal productWidthCm, decimal productHeightCm,
        string sellerId, FulfillmentMethod fulfillmentMethod)
    {
        if (price <= 0m)
            throw new ArgumentOutOfRangeException(
                nameof(price), "Product price cannot be 0 or negative.");

        if (availableStock > 0 && fulfillmentMethod == FulfillmentMethod.Fbb)
            throw new ManualInsertOfFbbStockNotSupportedException();
        if (subcategory is null)
            throw new ArgumentNullException(nameof(subcategory));

        var invalidArgName = productLengthCm <= 0m ? nameof(productLengthCm)
            : productWidthCm <= 0m ? nameof(productWidthCm)
            : productHeightCm <= 0m ? nameof(productHeightCm) : null;
        if (invalidArgName is not null)
        {
            throw new ArgumentOutOfRangeException(invalidArgName);
        }

        ProductName = productName;
        ProductDescription = productDescription;
        Price = price;
        AvailableStock = availableStock;
        Subcategory = subcategory;
        MainDepartment = Subcategory.MainDepartment;
        SubcategoryId = Subcategory.Id;
        MainDepartmentId = MainDepartment.Id;
        ProductLengthCm = productLengthCm;
        ProductWidthCm = productWidthCm;
        ProductHeightCm = productHeightCm;

        SellerId = sellerId;
        FulfillmentMethod = fulfillmentMethod;
        ListingStatus = availableStock > 0 && fulfillmentMethod == FulfillmentMethod.Merchant
            ? ListingStatus.Active : ListingStatus.InactiveOutOfStock;
    }

    [Newtonsoft.Json.JsonConstructor]
    private CatalogItem(
        string productName, string productDescription,
        decimal price, string imageUri, uint availableStock,
        int mainDepartmentId, int subcategoryId,
        decimal productLengthCm, decimal productWidthCm, decimal productHeightCm,
        string sellerId, FulfillmentMethod fulfillmentMethod, bool hasOrdersInProgress)
    {
        ProductName = productName;
        ProductDescription = productDescription;
        Price = price;
        ImageFilename = imageUri;
        AvailableStock = availableStock;
        MainDepartmentId = mainDepartmentId;
        SubcategoryId = subcategoryId;
        ProductLengthCm = productLengthCm;
        ProductWidthCm = productWidthCm;
        ProductHeightCm = productHeightCm;
        SellerId = sellerId;
        FulfillmentMethod = fulfillmentMethod;
        ListingStatus = availableStock > 0
            ? ListingStatus.Active : ListingStatus.InactiveOutOfStock;
        HasOrdersInProgress = hasOrdersInProgress;
    }

    // EF read constructor
    private CatalogItem() { }

    public void ChangeProductDetails(
        string? productName = null, string? productDescription = null,
        decimal? price = null, string? imageFilename = null)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Item deleted.");

        if (price <= 0m)
            throw new ArgumentOutOfRangeException(
                nameof(price), "Product price cannot be 0 or negative.");

        ProductName = productName ?? ProductName;
        ProductDescription = productDescription ?? ProductDescription;
        Price = price ?? Price;
        ImageFilename = imageFilename ?? ImageFilename;
    }

    public void ChangeProductDimensions(decimal length, decimal width, decimal height)
    {
        var invalidArgName = length <= 0m ? nameof(length)
            : width <= 0m ? nameof(width)
            : height <= 0m ? nameof(height) : null;
        if (invalidArgName is not null)
        {
            throw new ArgumentOutOfRangeException(invalidArgName);
        }
        ProductLengthCm = length;
        ProductWidthCm = width;
        ProductHeightCm = height;
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

    public void DeleteListing()
    {
        if (IsDeleted)
        {
            return;
        }
        // All orders must be completed in order to delete product
        if (HasOrdersInProgress)
        {
            throw new ProductHasOrdersInProgressException();
        }

        // If the product is fulfilled by Bazaar then all FBB stock must be moved out before deleting
        if (IsFbb && AvailableStock > 0)
        {
            throw new ProductFbbInventoryNotEmptyException();
        }
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

    public void UpdateHasOrdersInProgress(bool hasOrdersInProgress)
    {
        HasOrdersInProgress = hasOrdersInProgress;
    }

    public void ChangeSubcategory(ProductCategory category)
    {
        Subcategory = category;
        MainDepartment = Subcategory.MainDepartment;
        SubcategoryId = Subcategory.Id;
        MainDepartmentId = MainDepartment.Id;
    }
}
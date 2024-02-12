namespace Bazaar.Catalog.Web.Messages;

public record CatalogItemResponse
{
    public int Id { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public uint AvailableStock { get; set; }
    public CatalogItemDimensions Dimensions { get; set; }
    public CatalogItemCategoryResponse MainDepartment { get; set; }
    public CatalogItemCategoryResponse Subcategory { get; set; }
    public string SellerId { get; set; }
    public ListingStatus ListingStatus { get; set; }
    public FulfillmentMethod FulfillmentMethod { get; set; }
    public bool HasOrdersInProgress { get; set; }

    public CatalogItemResponse(CatalogItem item, string imageHostLocation)
    {
        Id = item.Id;
        ProductId = item.ProductId;
        ProductName = item.ProductName;
        ProductDescription = item.ProductDescription;
        Price = item.Price;
        ImageUrl = item.ImageFilename is not null
            ? Url.Combine(imageHostLocation, item.ImageFilename) : item.ImageFilename;
        AvailableStock = item.AvailableStock;
        Dimensions = new(item.ProductLengthCm, item.ProductWidthCm, item.ProductHeightCm);

        MainDepartment = new(item.MainDepartmentId, item.MainDepartment.Name);
        Subcategory = new(item.SubcategoryId, item.Subcategory.Name);
        SellerId = item.SellerId;
        ListingStatus = item.ListingStatus;
        FulfillmentMethod = item.FulfillmentMethod;
        HasOrdersInProgress = item.HasOrdersInProgress;
    }

    public record CatalogItemCategoryResponse(int CategoryId, string Name);
}

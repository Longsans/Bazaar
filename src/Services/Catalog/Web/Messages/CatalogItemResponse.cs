namespace Bazaar.Catalog.Web.Messages;

public record CatalogItemResponse
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public string ProductDescription { get; private set; }
    public decimal Price { get; private set; }
    public string? ImageUrl { get; private set; }
    public uint AvailableStock { get; private set; }
    public string SellerId { get; private set; }
    public ListingStatus ListingStatus { get; private set; }
    public FulfillmentMethod FulfillmentMethod { get; private set; }
    public bool HasOrdersInProgress { get; private set; }

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
        SellerId = item.SellerId;
        ListingStatus = item.ListingStatus;
        FulfillmentMethod = item.FulfillmentMethod;
        HasOrdersInProgress = item.HasOrdersInProgress;
    }
}

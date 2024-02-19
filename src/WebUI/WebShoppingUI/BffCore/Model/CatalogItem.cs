namespace WebShoppingUI.Model;

public class CatalogItem
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int AvailableStock { get; set; }
    public FulfillmentMethod FulfillmentMethod { get; set; }
    public string SellerId { get; set; }
}

public enum FulfillmentMethod
{
    Merchant,
    Fbb
}

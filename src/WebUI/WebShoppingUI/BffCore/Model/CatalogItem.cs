namespace WebShoppingUI.Model;

public class CatalogItem
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public decimal Price { get; set; }
    public int AvailableStock { get; set; }
    public string SellerId { get; set; }
}

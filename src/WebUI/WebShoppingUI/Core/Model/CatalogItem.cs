namespace WebShoppingUI.Model;

public class CatalogItem
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int AvailableStock { get; set; }
    public string SellerId { get; set; }
}

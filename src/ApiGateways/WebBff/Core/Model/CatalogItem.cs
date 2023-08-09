namespace Bazaar.ApiGateways.WebBff.Core.Model;

public class CatalogItem
{
    public int Id { get; set; }
    public string ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int AvailableStock { get; set; }

    public string SellerId { get; set; }

    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
}

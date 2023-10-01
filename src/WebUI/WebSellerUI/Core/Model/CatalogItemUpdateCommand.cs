namespace WebSellerUI.Model;

public class CatalogItemUpdateCommand
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int AvailableStock { get; set; }

    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
}

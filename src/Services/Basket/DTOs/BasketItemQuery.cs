namespace Bazaar.Basket.DTOs;

public class BasketItemQuery
{
    public int Id { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public uint Quantity { get; set; }
    public string ImageUrl { get; set; }
    public int BasketId { get; set; }

    public BasketItemQuery(BasketItem item)
    {
        Id = item.Id;
        ProductId = item.ProductId;
        ProductName = item.ProductName;
        UnitPrice = item.UnitPrice;
        Quantity = item.Quantity;
        ImageUrl = item.ImageUrl;
        BasketId = item.BasketId;
    }
}
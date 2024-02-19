namespace WebShoppingUI.Model;

public class BasketItem
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public uint Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public int BasketId { get; set; }
}

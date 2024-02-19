namespace WebShoppingUI.Model;

public class Basket
{
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; }
    public decimal Total { get; set; }
}

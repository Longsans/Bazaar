namespace Basket.Model;

public class CustomerBasket
{
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; } = new();

    public CustomerBasket() { }
    public CustomerBasket(string buyerId, params BasketItem[] items)
    {
        BuyerId = buyerId;
        Items.AddRange(items);
    }
}
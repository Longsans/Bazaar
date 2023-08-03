namespace Bazaar.Basket.Core.Model;

public class BuyerBasket
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; }

    public BuyerBasket() { }
    public BuyerBasket(string buyerId, params BasketItem[] items)
    {
        BuyerId = buyerId;
        Items = new(items);
    }
}
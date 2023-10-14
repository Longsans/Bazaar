namespace Bazaar.Basket.Web.Messages;

public class BuyerBasketQuery
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<BasketItemQuery> Items { get; set; }
    public decimal Total { get; set; }

    public BuyerBasketQuery(BuyerBasket basket)
    {
        Id = basket.Id;
        BuyerId = basket.BuyerId;
        Items = basket.Items.Select(i => new BasketItemQuery(i)).ToList();
        Total = basket.Total;
    }
}

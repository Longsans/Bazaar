namespace Bazaar.ApiGateways.WebBff.Core.Model;

public class Basket
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<BasketItem> Items { get; set; }
}

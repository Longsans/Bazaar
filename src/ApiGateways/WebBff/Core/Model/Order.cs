namespace Bazaar.ApiGateways.WebBff.Core.Model;

public class Order
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public string Status { get; set; }
}

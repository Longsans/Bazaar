namespace Bazaar.ApiGateways.WebBff.Core.Model;

public class OrderCreateCommand
{
    public string BuyerId { get; set; }
    public List<OrderItemCreateCommand> Items { get; set; } = new();
}
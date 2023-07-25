namespace Bazaar.ApiGateways.WebBff.Core.Model;

public class OrderCreateCommand
{
    public string BuyerExternalId { get; set; }
    public List<OrderItemCreateCommand> Items { get; set; } = new();
}
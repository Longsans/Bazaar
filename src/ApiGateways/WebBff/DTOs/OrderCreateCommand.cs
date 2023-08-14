namespace Bazaar.ApiGateways.WebBff.DTOs;

public record OrderCreateCommand(
    string BuyerId,
    List<OrderItemCreateCommand> Items)
{
    public OrderCreateCommand(Order order) :
        this(
            order.BuyerId,
            order.Items.Select(item => new OrderItemCreateCommand(item)).ToList())
    { }
}

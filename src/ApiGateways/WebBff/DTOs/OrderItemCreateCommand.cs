namespace Bazaar.ApiGateways.WebBff.DTOs;

public record OrderItemCreateCommand(
    string ProductId,
    string ProductName,
    decimal ProductUnitPrice,
    uint Quantity)
{
    public OrderItemCreateCommand(OrderItem item) : this(item.ProductId, item.ProductName, item.ProductUnitPrice, item.Quantity) { }
}

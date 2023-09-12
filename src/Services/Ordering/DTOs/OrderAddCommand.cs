namespace Bazaar.Ordering.DTOs;

public class OrderAddCommand
{
    public string BuyerId { get; set; }
    public List<OrderItemAddCommand> Items { get; set; } = new();
    public string ShippingAddress { get; set; }

    public Order ToOrder() => new()
    {
        BuyerId = BuyerId,
        Items = Items.Select(i => i.ToOrderItem()).ToList(),
        ShippingAddress = ShippingAddress
    };
}
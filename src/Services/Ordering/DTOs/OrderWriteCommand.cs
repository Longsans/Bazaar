namespace Bazaar.Ordering.Dto;

public class OrderWriteCommand
{
    public string BuyerId { get; set; }
    public List<OrderItemWriteCommand> Items { get; set; } = new();

    public Order ToOrder() => new()
    {
        BuyerId = BuyerId,
        Items = Items.Select(i => i.ToOrderItem()).ToList()
    };
}
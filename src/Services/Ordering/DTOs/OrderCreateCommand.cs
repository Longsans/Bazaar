namespace Bazaar.Ordering.Dto;

public class OrderCreateCommand
{
    public string BuyerExternalId { get; set; }
    public List<OrderItemCreateCommand> Items { get; set; } = new();
}
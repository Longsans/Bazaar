namespace Bazaar.Ordering.Model;

public class Order
{
    public int Id { get; set; }
    public string ExternalId { get; set; }
    public string BuyerExternalId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public OrderStatus Status { get; set; }

    public Order() { }

    public Order(OrderCreateCommand command)
    {
        BuyerExternalId = command.BuyerExternalId;
        Items = command.Items.Select(i => new OrderItem(i)).ToList();
    }

    public void AssignExternalId()
    {
        ExternalId = $"ORDR-{Id}";
    }
}
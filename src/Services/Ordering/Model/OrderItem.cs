namespace Bazaar.Ordering.Model;

public class OrderItem
{
    public int Id { get; set; }
    public string ProductExternalId { get; set; }
    public string ProductName { get; set; }
    public decimal ProductUnitPrice { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public OrderItem() { }

    public OrderItem(OrderItemCreateCommand command)
    {
        ProductExternalId = command.ProductExternalId;
        ProductName = command.ProductName;
        ProductUnitPrice = command.ProductUnitPrice;
        Quantity = command.Quantity;
    }
}
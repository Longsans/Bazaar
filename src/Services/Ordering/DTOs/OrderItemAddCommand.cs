namespace Bazaar.Ordering.DTOs;

public class OrderItemAddCommand
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal ProductUnitPrice { get; set; }
    public uint Quantity { get; set; }

    public OrderItem ToOrderItem() => new()
    {
        ProductId = ProductId,
        ProductName = ProductName,
        ProductUnitPrice = ProductUnitPrice,
        Quantity = Quantity,
    };
}
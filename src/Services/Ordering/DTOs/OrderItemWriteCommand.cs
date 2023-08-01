namespace Bazaar.Ordering.Dto;

public class OrderItemWriteCommand
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal ProductUnitPrice { get; set; }
    public int Quantity { get; set; }

    public OrderItem ToOrderItem() => new()
    {
        ProductId = ProductId,
        ProductName = ProductName,
        ProductUnitPrice = ProductUnitPrice,
        Quantity = Quantity,
    };
}
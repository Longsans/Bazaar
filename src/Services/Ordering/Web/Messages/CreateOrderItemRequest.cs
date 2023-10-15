namespace Bazaar.Ordering.Web.Messages;

public class CreateOrderItemRequest
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal ProductUnitPrice { get; set; }
    public uint Quantity { get; set; }
}
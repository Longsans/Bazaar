using Newtonsoft.Json;

namespace Bazaar.Ordering.Domain.Entites;

public class OrderItem
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal ProductUnitPrice { get; private set; }
    public uint Quantity { get; private set; }
    public Order Order { get; private set; }
    public int OrderId { get; private set; }

    [JsonConstructor]
    public OrderItem(
        int id, string productId, string productName,
        decimal productUnitPrice, uint quantity, int orderId)
    {
        Id = id;
        ProductId = productId;
        ProductName = productName;
        ProductUnitPrice = productUnitPrice;
        Quantity = quantity;
        OrderId = orderId;
    }

    public OrderItem(
        string productId, string productName,
        decimal productUnitPrice, uint quantity, int orderId)
    {
        ProductId = productId;
        ProductName = productName;
        ProductUnitPrice = productUnitPrice;
        Quantity = quantity;
        OrderId = orderId;
    }
}
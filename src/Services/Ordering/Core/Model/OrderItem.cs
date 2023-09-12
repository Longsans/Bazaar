namespace Bazaar.Ordering.Core.Model;

public class OrderItem
{
    public int Id { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal ProductUnitPrice { get; set; }
    public uint Quantity { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
}
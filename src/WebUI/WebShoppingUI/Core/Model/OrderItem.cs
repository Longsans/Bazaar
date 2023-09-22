namespace WebShoppingUI.Model;

public class OrderItem
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal ProductUnitPrice { get; set; }
    public uint Quantity { get; set; }
}

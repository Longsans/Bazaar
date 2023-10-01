namespace WebShoppingUI.DTOs;

public class OrderAddCommand
{
    public string BuyerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public string ShippingAddress { get; set; }
}

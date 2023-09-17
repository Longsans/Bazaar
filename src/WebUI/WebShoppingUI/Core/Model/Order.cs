namespace WebShoppingUI.Model;

public class Order
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public string ShippingAddress { get; set; }
    public string Status { get; set; }
}

public enum OrderStatus
{
    AwaitingValidation = 1,
    ProcessingPayment = 2,
    AwaitingSellerConfirmation = 4,
    Shipping = 8,
    Shipped = 16,
    Cancelled = 32,
    Postponed = 64,
}
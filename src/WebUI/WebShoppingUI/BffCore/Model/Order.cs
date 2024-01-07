namespace WebShoppingUI.Model;

public class Order
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public string ShippingAddress { get; set; }
    public OrderStatus Status { get; set; }
    public string? CancelReason { get; set; }
}

public enum OrderStatus
{
    PendingValidation = 1,
    ProcessingPayment = 2,
    PendingSellerConfirmation = 4,
    Shipping = 8,
    Shipped = 16,
    Cancelled = 32,
    Postponed = 64,
}
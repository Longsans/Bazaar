namespace WebSellerUI.Model;

public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal Total { get; set; }
    public string ShippingAddress { get; set; }
    public string BuyerId { get; set; }
    public OrderStatus Status { get; set; }
    public string? CancelReason { get; set; }
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
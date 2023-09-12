namespace WebSellerUI.Model;

public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal Total { get; set; }
    public string ShippingAddress { get; set; }
    public string BuyerId { get; set; }
    public OrderStatus Status { get; set; }
}

public enum OrderStatus
{
    AwaitingValidation,
    ProcessingPayment,
    AwaitingSellerConfirmation,
    Shipping,
    Shipped,
    Cancelled,
    Postponed,
}
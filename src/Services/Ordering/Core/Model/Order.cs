namespace Bazaar.Ordering.Core.Model;

public class Order
{
    public int Id { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public string ShippingAddress { get; set; }
    public string BuyerId { get; set; }
    public OrderStatus Status { get; set; }

    #region Domain logic

    public bool IsCancellable
        => Status.HasFlag(OrderStatus.AwaitingSellerConfirmation)
        || Status.HasFlag(OrderStatus.Postponed);

    #endregion
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
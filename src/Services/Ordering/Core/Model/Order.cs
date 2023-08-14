namespace Bazaar.Ordering.Core.Model;

public class Order
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public string ShippingAddress { get; set; }
    public OrderStatus Status { get; set; }

    #region Domain logic

    public bool IsCancellable
        => Status == OrderStatus.AwaitingSellerConfirmation || Status == OrderStatus.Postponed;

    #endregion
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
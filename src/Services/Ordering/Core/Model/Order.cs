namespace Bazaar.Ordering.Core.Model;

public class Order
{
    public int Id { get; set; }
    public string BuyerId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public OrderStatus Status { get; set; }

    #region Domain logic

    public bool IsCancellable
        => Status == OrderStatus.AwaitingSellerConfirmation || Status == OrderStatus.Postponed;

    #endregion
}

public enum OrderStatus
{
    ProcessingPayment,
    AwaitingSellerConfirmation,
    Shipping,
    Shipped,
    Cancelled,
    Postponed,
}
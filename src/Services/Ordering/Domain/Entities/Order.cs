using Newtonsoft.Json;

namespace Bazaar.Ordering.Domain.Entites;

public class Order
{
    public int Id { get; private set; }

    private readonly List<OrderItem> _items;
    public IReadOnlyCollection<OrderItem> Items => _items;

    public string ShippingAddress { get; private set; }
    public decimal Total { get; private set; }
    public string BuyerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? CancelReason { get; private set; }

    public Order(string shippingAddress, string buyerId, IEnumerable<OrderItem> items)
    {
        if (!items.Any())
            throw new ArgumentException("Order has no items.", nameof(items));

        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new ArgumentException("Shipping address cannot be empty", nameof(shippingAddress));

        ShippingAddress = shippingAddress;
        BuyerId = buyerId;
        _items = items.ToList();
        Status = OrderStatus.AwaitingValidation;
    }

    [JsonConstructor]
    private Order(
        int id, string shippingAddress, string buyerId, OrderStatus status)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new ArgumentException("Shipping address cannot be empty", nameof(shippingAddress));

        Id = id;
        _items = new();
        ShippingAddress = shippingAddress;
        BuyerId = buyerId;
        Status = status;
    }

    #region Domain logic

    public bool IsCancellable
        => Status.HasFlag(OrderStatus.AwaitingSellerConfirmation)
        || Status.HasFlag(OrderStatus.Postponed);

    public void Cancel(string reason)
    {
        Status = IsCancellable
            ? OrderStatus.Cancelled
            : throw new InvalidOperationException(
                "Order can only be cancelled if it is currently awaiting seller's confirmation or being postponed.");
        CancelReason = reason;
    }

    public void StartPayment()
    {
        Status = Status == OrderStatus.AwaitingValidation
            ? OrderStatus.ProcessingPayment
            : throw new InvalidOperationException(
                "Can only start payment if order was previously awaiting validation.");
    }

    public void AwaitSellerConfirmation()
    {
        Status = Status == OrderStatus.ProcessingPayment
            ? OrderStatus.AwaitingSellerConfirmation
            : throw new InvalidOperationException(
                "Can only request seller confirmation if order's payment was previously under process.");
    }

    public void Ship()
    {
        Status = Status == OrderStatus.AwaitingSellerConfirmation
            ? OrderStatus.Shipping
            : throw new InvalidOperationException(
                "Can only start shipment if order was previously awaiting seller's confirmation.");
    }

    public void ConfirmShipped()
    {
        Status = Status == OrderStatus.Shipping
            ? OrderStatus.Shipped
            : throw new InvalidOperationException(
                "Can only confirm shipped for a shipping order.");
    }

    public void Postpone()
    {
        Status = Status != OrderStatus.Cancelled && Status != OrderStatus.Shipped
            ? OrderStatus.Postponed
            : throw new InvalidOperationException(
                "Can only postpone order if order is not shipped nor cancelled.");
    }

    public void UpdateTotal()
    {
        Total = _items.Sum(x => x.Quantity * x.ProductUnitPrice);
    }

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
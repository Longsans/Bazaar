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

    public bool IsAllStocksConfirmed => Items.All(x => x.Status == OrderItemStatus.StockConfirmed);
    //public bool IsAllItemsSellerConfirmed => Items.All(x => x.Status == OrderItemStatus.SellerConfirmed);
    public bool CanProceedToPayment => Status == OrderStatus.PendingValidation && IsAllStocksConfirmed;

    // Create constructor
    public Order(string shippingAddress, string buyerId, IEnumerable<OrderItem> items)
    {
        if (!items.Any())
            throw new ArgumentException("Order has no items.", nameof(items));

        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new ArgumentNullException(nameof(shippingAddress), "Shipping address cannot be empty");

        var duplicateProductIds = new List<string>();
        foreach (var productIdGroup in items.GroupBy(item => item.ProductId))
        {
            if (productIdGroup.Count() > 1)
            {
                duplicateProductIds.Add(productIdGroup.Key);
            }
        }

        if (duplicateProductIds.Any())
            throw new DuplicateProductsException(duplicateProductIds);

        ShippingAddress = shippingAddress;
        BuyerId = buyerId;
        _items = items.ToList();
        Status = OrderStatus.PendingValidation;
    }

    [JsonConstructor]
    private Order(string buyerId, string shippingAddress, OrderStatus status)
    {
        BuyerId = buyerId;
        ShippingAddress = shippingAddress;
        Status = status;
        _items = new List<OrderItem>();
    }

    // EF requires this to read from DB correctly, otherwise the damn thing tries to use
    // the create constructor and fails to provide order items
    private Order() { }

    #region Domain logic

    public bool IsCancellable
        => Status == OrderStatus.PendingSellerConfirmation
        || Status == OrderStatus.Postponed;

    public void Cancel(string reason)
    {
        Status = IsCancellable
            ? OrderStatus.Cancelled
            : throw new InvalidOperationException(
                "Order can only be cancelled if it is currently pending seller's confirmation or being postponed.");
        CancelReason = reason;
    }

    public void StartPayment()
    {
        if (!CanProceedToPayment)
        {
            throw new InvalidOperationException(
                "Can only start payment if all items' stocks have been confirmed " +
                "and order is pending validation.");
        }
        Status = OrderStatus.ProcessingPayment;
    }

    public void RequestSellerConfirmation()
    {
        Status = Status == OrderStatus.ProcessingPayment
            ? OrderStatus.PendingSellerConfirmation
            : throw new InvalidOperationException(
                "Can only request seller confirmation if order's payment was previously under process.");
    }

    public void Ship()
    {
        Status = Status == OrderStatus.PendingSellerConfirmation // && IsAllItemsSellerConfirmed
            ? OrderStatus.Shipping
            : throw new InvalidOperationException(
                "Can only start shipment if order was previously pending seller's confirmation.");
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
        Status = Status == OrderStatus.Shipping
                || Status == OrderStatus.PendingSellerConfirmation
                || Status == OrderStatus.ProcessingPayment
            ? OrderStatus.Postponed
            : throw new InvalidOperationException(
                "Can only postpone order if order is currently processing payment, " +
                "pending seller confirmation, or shipping.");
    }

    public void UpdateTotal()
    {
        Total = _items.Sum(x => x.Quantity * x.ProductUnitPrice);
    }

    #endregion
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
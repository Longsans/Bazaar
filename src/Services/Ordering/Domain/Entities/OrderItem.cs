namespace Bazaar.Ordering.Domain.Entites;

public class OrderItem
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal ProductUnitPrice { get; private set; }
    public uint Quantity { get; private set; }
    public OrderItemStatus Status { get; private set; }
    public Order Order { get; private set; }
    public int OrderId { get; private set; }

    [Newtonsoft.Json.JsonConstructor]
    public OrderItem(
        int id, string productId, string productName,
        decimal productUnitPrice, uint quantity, int orderId)
        : this(productId, productName, productUnitPrice, quantity, orderId)
    {
        Id = id;
    }

    public OrderItem(
        string productId, string productName,
        decimal productUnitPrice, uint quantity, int orderId)
    {
        if (productUnitPrice <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(productUnitPrice), "Product unit price must be greater than 0.");
        }

        if (quantity == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity), "Quantity must be larger than 0.");
        }

        ProductId = productId;
        ProductName = productName;
        ProductUnitPrice = productUnitPrice;
        Quantity = quantity;
        OrderId = orderId;
        Status = OrderItemStatus.PendingStock;
    }

    public void SetStockConfirmed()
    {
        Status = Status == OrderItemStatus.PendingStock
            ? OrderItemStatus.StockConfirmed
            : throw new InvalidOperationException("Stock status has already been determined.");
    }

    public void SetStockRejected()
    {
        Status = Status == OrderItemStatus.PendingStock
            ? OrderItemStatus.StockRejected
            : throw new InvalidOperationException("Stock status has already been determined.");
    }
}

public enum OrderItemStatus
{
    PendingStock,
    StockConfirmed,
    StockRejected
}
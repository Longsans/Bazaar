namespace Bazaar.Ordering.Repositories;

public class OrderRepository : IOrderRepository
{
    private List<Order> _orders;
    private const string ORDER_ITEMS_SECTION = "orderItems";
    public int NextOrderId => _orders.Count + 1;

    public OrderRepository(JsonDataAdapter adapter)
    {
        var items = adapter.ReadToObjects<OrderItem>(ORDER_ITEMS_SECTION, (item, id) => item.Id = id);

        _orders = new()
        {
            new Order {
                BuyerExternalId = "CUST-1",
                Items = items.ToList(),
                Status = OrderStatus.Postponed,
            }
        };
        _orders = adapter.GenerateId(_orders, (o, id) => o.Id = id).ToList();
    }

    public Order CreateProcessingPayment(Order order)
    {
        if (_orders.Any(o => o.Id == order.Id))
            throw new ArgumentException("Order already created");
        order.Id = NextOrderId;
        order.Status = OrderStatus.ProcessingPayment;
        _orders.Add(order);
        return order;
    }

    public Order CreateShippingOrder(Order order)
    {
        if (_orders.Any(o => o.Id == order.Id))
            throw new ArgumentException("Order already created");
        order.Id = NextOrderId;
        order.Status = OrderStatus.Shipping;
        _orders.Add(order);
        return order;
    }

    public Order? GetById(int id)
    {
        return _orders.FirstOrDefault(o => o.Id == id);
    }

    public void Update(Order order)
    {
        var existing = _orders.FirstOrDefault(o => o.Id == order.Id);
        if (existing == null)
            return;
        _orders.Remove(existing);
        _orders.Add(order);
    }

    public void UpdateStatus(int orderId, OrderStatus status)
    {
        var existing = _orders.FirstOrDefault(o => o.Id == orderId);
        if (existing == null)
            return;
        existing.Status = status;
    }
}
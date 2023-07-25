namespace Bazaar.Ordering.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly List<Order> _orders;
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
        _orders.ForEach(o => o.AssignExternalId());
    }

    public Order CreateProcessingPayment(Order order)
    {
        if (_orders.Any(o => o.Id == order.Id))
            throw new ArgumentException("Order already created");
        order.Id = NextOrderId;
        order.AssignExternalId();
        order.Status = OrderStatus.ProcessingPayment;
        _orders.Add(order);
        return order;
    }

    public Order CreateShippingOrder(Order order)
    {
        if (_orders.Any(o => o.Id == order.Id))
            throw new ArgumentException("Order already created");
        order.Id = NextOrderId;
        order.AssignExternalId();
        order.Status = OrderStatus.Shipping;
        _orders.Add(order);
        return order;
    }

    public Order? GetById(int id)
    {
        return _orders.FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<Order> GetAll()
    {
        return _orders;
    }

    public Order? GetLatest()
    {
        return _orders.Count > 0 ? _orders[_orders.Count - 1] : null;
    }

    public void Update(Order order)
    {
        var existing = _orders.FirstOrDefault(o => o.Id == order.Id);
        if (existing == null)
            return;
        existing.Items = order.Items;
        existing.Status = order.Status;
    }

    public void UpdateStatus(int orderId, OrderStatus status)
    {
        var existing = _orders.FirstOrDefault(o => o.Id == orderId);
        if (existing == null)
            return;
        existing.Status = status;
    }
}
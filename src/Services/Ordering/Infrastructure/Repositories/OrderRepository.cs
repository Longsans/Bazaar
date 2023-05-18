namespace Bazaar.Ordering.Repositories;

public class OrderRepository : IOrderRepository
{
    private List<Order> _orders;
    private int _nextOrderId => _orders.Count + 1;

    public OrderRepository()
    {
        var items = new OrderItem[] {
            new OrderItem {
                Id = 1,
                ProductExternalId = "PROD-1",
                ProductName = "The Winds of Winter",
                ProductUnitPrice = 34.99m,
                Quantity = 1,
                OrderId = 1,
            },
            new OrderItem {
                Id = 2,
                ProductExternalId = "PROD-2",
                ProductName = "A Dream of Spring",
                ProductUnitPrice = 45.99m,
                Quantity = 2,
                OrderId = 1,
            },
        };
        _orders = new()
        {
            new Order {
                Id = 1,
                BuyerExternalId = "CUST-1",
                Items = items.ToList(),
                Status = OrderStatus.Postponed,
            }
        };
    }

    public Order CreateProcessingPayment(Order order)
    {
        if (_orders.Any(o => o.Id == order.Id))
            throw new ArgumentException("Order already created");
        order.Id = _nextOrderId;
        order.Status = OrderStatus.ProcessingPayment;
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
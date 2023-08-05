namespace Bazaar.Ordering.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _context;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(OrderingDbContext context, ILogger<OrderRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Order? GetById(int id)
    {
        return _context.Orders
            .Include(o => o.Items)
            .FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<Order> GetAll()
    {
        return _context.Orders.Include(o => o.Items);
    }

    public Order CreateProcessingPaymentOrder(Order order)
    {
        order.Status = OrderStatus.ProcessingPayment;
        _context.Orders.Add(order);
        _context.SaveChanges();
        return order;
    }

    public IUpdateOrderStatusResult UpdateStatus(int id, OrderStatus status)
    {
        var order = _context.Orders
            .Include(o => o.Items)
            .FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return IUpdateOrderStatusResult.OrderNotFoundError;
        }

        if (status == OrderStatus.Cancelled && !order.IsCancellable)
        {
            return IUpdateOrderStatusResult.InvalidOrderCancellationError;
        }

        order.Status = status;
        _context.SaveChanges();
        return IUpdateOrderStatusResult.Success(order);
    }
}
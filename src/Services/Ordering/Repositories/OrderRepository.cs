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

    public Order? UpdateStatus(int id, OrderStatus status)
    {
        var order = _context.Orders
            .Include(o => o.Items)
            .FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return null;
        }

        if (status == OrderStatus.Cancelled && !order.IsCancellable)
        {
            _logger.LogWarning(
                "Logical error: Attempted to cancel an order that is either " +
                "(1) processing payment, " +
                "(2) shipping, " +
                "(3) shipped or " +
                "(4) already cancelled");
            return null;
        }

        order.Status = status;
        _context.SaveChanges();
        return order;
    }
}
namespace Bazaar.Ordering.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _context;

    public OrderRepository(OrderingDbContext context)
    {
        _context = context;
    }

    public Order? GetById(int id)
    {
        return _context.Orders
            .Include(o => o.Items)
            .FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<Order> GetByBuyerId(string buyerId, OrderStatus status = 0)
    {
        return _context.Orders
            .Include(o => o.Items)
            .Where(o => o.BuyerId == buyerId && o.Status.HasFlag(status));
    }

    public IEnumerable<Order> GetByProductId(string productId, OrderStatus status = 0)
    {
        return _context.Orders
            .Include(o => o.Items)
            .Where(o => o.Items.Any(item => item.ProductId == productId) && o.Status.HasFlag(status));
    }

    public IEnumerable<Order> GetAll()
    {
        return _context.Orders.Include(o => o.Items);
    }

    public ICreateOrderResult Create(Order order)
    {
        if (!order.Items.Any())
        {
            return ICreateOrderResult.OrderHasNoItemsError;
        }

        order.Status = OrderStatus.AwaitingValidation;
        _context.Orders.Add(order);
        _context.SaveChanges();

        return ICreateOrderResult.Success(order);
    }

    public IUpdateOrderStatusResult UpdateStatus(int id, OrderStatus status, string? cancelReason = null)
    {
        var order = _context.Orders
            .Include(o => o.Items)
            .FirstOrDefault(o => o.Id == id);

        if (order == null)
            return IUpdateOrderStatusResult.OrderNotFoundError;

        if (status == OrderStatus.Cancelled && !order.IsCancellable)
            return IUpdateOrderStatusResult.InvalidOrderCancellationError;

        if (status != OrderStatus.Cancelled && order.Status == OrderStatus.Cancelled)
            return IUpdateOrderStatusResult.InvalidStatusCancelledOrderError;

        order.Status = status;
        order.CancelReason = status == OrderStatus.Cancelled ? cancelReason : null;
        _context.SaveChanges();
        return IUpdateOrderStatusResult.Success(order);
    }
}
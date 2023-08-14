namespace Bazaar.Ordering.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _context;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(OrderingDbContext context, IEventBus eventBus, ILogger<OrderRepository> logger)
    {
        _context = context;
        _eventBus = eventBus;
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
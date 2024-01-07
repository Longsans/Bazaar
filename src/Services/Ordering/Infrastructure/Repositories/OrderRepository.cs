namespace Bazaar.Ordering.Infrastructure.Repositories;

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

    public IEnumerable<Order> GetByBuyerId(string buyerId)
    {
        return _context.Orders
            .Include(o => o.Items)
            .Where(o => o.BuyerId == buyerId);
    }

    public IEnumerable<Order> GetContainsProduct(string productId)
    {
        return _context.Orders
            .Include(o => o.Items)
            .Where(o => o.Items.Any(item => item.ProductId == productId));
    }

    public Order Create(Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
        return order;
    }

    public void Update(Order update)
    {
        _context.Orders.Update(update);
        _context.SaveChanges();
    }

    public void Delete(Order order)
    {
        _context.Orders.Remove(order);
        _context.SaveChanges();
    }
}
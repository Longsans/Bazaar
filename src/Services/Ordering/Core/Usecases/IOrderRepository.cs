namespace Bazaar.Ordering.Core.Usecases;

public interface IOrderRepository
{
    Order? GetById(int id);
    IEnumerable<Order> GetByBuyerId(string buyerId, OrderStatus status = 0);
    IEnumerable<Order> GetByProductId(string productId, OrderStatus status = 0);
    IEnumerable<Order> GetAll();
    ICreateOrderResult Create(Order order);
    IUpdateOrderStatusResult UpdateStatus(int id, OrderStatus status);
}
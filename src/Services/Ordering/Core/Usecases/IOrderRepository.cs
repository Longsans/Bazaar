namespace Bazaar.Ordering.Core.Usecases;

public interface IOrderRepository
{
    Order? GetById(int id);
    IEnumerable<Order> GetAll();
    ICreateOrderResult Create(Order order);
    IUpdateOrderStatusResult UpdateStatus(int id, OrderStatus status);
}
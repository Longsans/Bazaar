namespace Bazaar.Ordering.Core.Usecases;

public interface IOrderRepository
{
    Order? GetById(int id);
    IEnumerable<Order> GetAll();
    Order CreateProcessingPaymentOrder(Order order);
    IUpdateOrderStatusResult UpdateStatus(int id, OrderStatus status);
}
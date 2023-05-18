namespace Bazaar.Ordering.Model;

public interface IOrderRepository
{
    Order? GetById(int id);
    Order CreateProcessingPayment(Order order);
    void Update(Order order);
    void UpdateStatus(int orderId, OrderStatus status);
}
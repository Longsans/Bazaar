namespace Bazaar.Ordering.Core.Usecases;

public interface IOrderRepository
{
    // Band-aid method to get the newly created order's id back to the caller.
    // The proper way to do this would be to get the id AFTER transactions commit, from the DB.
    // I have yet to come up with an idea to implement such solution.
    public int NextOrderId { get; }

    Order? GetById(int id);
    IEnumerable<Order> GetAll();
    Order? GetLatest();
    Order CreateProcessingPayment(Order order);
    Order CreateShippingOrder(Order order);
    void Update(Order order);
    void UpdateStatus(int orderId, OrderStatus status);
}
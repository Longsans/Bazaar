namespace Bazaar.Ordering.Domain.Interfaces;

public interface IOrderRepository
{
    Order? GetById(int id);
    IEnumerable<Order> GetByBuyerId(string buyerId);
    IEnumerable<Order> GetContainsProduct(string productId);
    Order Create(Order order);
    void Update(Order update);
    void Delete(Order order);
}
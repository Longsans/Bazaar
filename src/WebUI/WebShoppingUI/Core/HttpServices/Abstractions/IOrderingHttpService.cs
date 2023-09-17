namespace WebShoppingUI.HttpServices;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;
using OrderResult = ServiceCallResult<Order>;

public interface IOrderingHttpService
{
    Task<OrderCollectionResult> GetOrdersByBuyerId(string buyerId);
    Task<OrderResult> UpdateStatus(int orderId, OrderStatus status);
}

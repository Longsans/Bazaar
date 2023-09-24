namespace WebShoppingUI.DataServices;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;
using OrderResult = ServiceCallResult<Order>;

public interface IOrderingDataService
{
    Task<OrderCollectionResult> GetOrdersByBuyerId(string buyerId);
    Task<OrderResult> UpdateStatus(int orderId, OrderUpdateStatusCommand updateCommand);
}

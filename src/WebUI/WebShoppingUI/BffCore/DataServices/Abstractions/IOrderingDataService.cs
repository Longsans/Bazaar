namespace WebShoppingUI.DataServices;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;

public interface IOrderingDataService
{
    Task<OrderCollectionResult> GetOrdersByBuyerId(string buyerId);
    Task<OrderCollectionResult> GetOrdersByBuyerIdContainingProducts(string buyerId, string[] productIds);
    Task<ServiceCallResult> UpdateStatus(int orderId, OrderUpdateStatusCommand updateCommand);
}

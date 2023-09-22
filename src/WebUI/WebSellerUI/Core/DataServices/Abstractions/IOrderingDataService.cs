namespace WebSellerUI.DataServices;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;

public interface IOrderingDataService
{
    Task<OrderCollectionResult> GetByProductIds(string productIds);
    Task<ServiceCallResult> UpdateStatus(int orderId, OrderStatus status);
}

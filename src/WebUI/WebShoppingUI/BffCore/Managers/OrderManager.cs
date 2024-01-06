namespace WebShoppingUI.Managers;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;

public class OrderManager
{
    private readonly IOrderingDataService _orderingService;

    public OrderManager(IOrderingDataService orderingService)
    {
        _orderingService = orderingService;
    }

    public async Task<OrderCollectionResult> GetByBuyerIdAsync(string buyerId)
    {
        return await _orderingService.GetOrdersByBuyerId(buyerId);
    }


    public async Task<OrderCollectionResult> GetByBuyerIdContainingProductsAsync(string buyerId, string[] productIds)
    {
        return await _orderingService.GetOrdersByBuyerIdContainingProducts(buyerId, productIds);
    }

    public async Task<ServiceCallResult> CancelOrder(int orderId, string reason)
    {
        var updateCommand = new OrderUpdateStatusCommand
        {
            Status = OrderStatus.Cancelled,
            CancelReason = reason
        };
        return await _orderingService.UpdateStatus(orderId, updateCommand);
    }
}

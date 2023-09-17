namespace WebShoppingUI.Managers;

using OrderCollectionResult = ServiceCallResult<IEnumerable<Order>>;
using OrderResult = ServiceCallResult<Order>;

public class OrderManager
{
    private readonly IOrderingHttpService _orderingService;

    public OrderManager(IOrderingHttpService orderingService)
    {
        _orderingService = orderingService;
    }

    public async Task<OrderCollectionResult> GetByBuyerIdAsync(string buyerId)
    {
        return await _orderingService.GetOrdersByBuyerId(buyerId);
    }

    public async Task<OrderResult> CancelOrder(int orderId)
    {
        return await _orderingService.UpdateStatus(orderId, OrderStatus.Cancelled);
    }
}

namespace Bazaar.Ordering.Domain.Interfaces;

public interface IHandleOrderService
{
    Order PlaceOrder(Order order);
    Result UpdateOrderStatus(int orderId, OrderStatus status, string? cancelReason = null);
    void PublishProductOrdersStatusReport(string productId);
}

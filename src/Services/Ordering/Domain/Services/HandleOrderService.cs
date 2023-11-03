namespace Bazaar.Ordering.Domain.Services;

public class HandleOrderService : IHandleOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;

    public HandleOrderService(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    public Order PlaceOrder(Order order)
    {
        _orderRepository.Create(order);
        foreach (var productId in order.Items.Select(x => x.ProductId))
        {
            PublishProductOrdersStatusReport(productId);
        }
        return order;
    }

    public Result UpdateOrderStatus(int orderId, OrderStatus status, string? cancelReason = null)
    {
        var order = _orderRepository.GetById(orderId);
        if (order == null)
        {
            return Result.NotFound("Order not found.");
        }

        try
        {
            switch (status)
            {
                case OrderStatus.ProcessingPayment:
                    order.StartPayment();
                    break;
                case OrderStatus.AwaitingSellerConfirmation:
                    order.AwaitSellerConfirmation();
                    break;
                case OrderStatus.Shipping:
                    order.Ship();
                    break;
                case OrderStatus.Shipped:
                    order.ConfirmShipped();
                    break;

                case OrderStatus.Postponed:
                    order.Postpone();
                    break;

                case OrderStatus.Cancelled when cancelReason != null:
                    order.Cancel(cancelReason);
                    break;
                case OrderStatus.Cancelled when cancelReason == null:
                    return Result.Invalid(new ValidationError()
                    {
                        Identifier = nameof(cancelReason),
                        ErrorMessage = "Order cancellation reason is required."
                    });

                default:
                    return Result.Invalid(new ValidationError()
                    {
                        Identifier = nameof(status),
                        ErrorMessage = "Undefined order status."
                    });
            }
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }

        _orderRepository.Update(order);
        if (status == OrderStatus.Shipped || status == OrderStatus.Cancelled)
        {
            foreach (var productId in order.Items.Select(x => x.ProductId))
            {
                PublishProductOrdersStatusReport(productId);
            }
        }

        return Result.Success();
    }

    public void PublishProductOrdersStatusReport(string productId)
    {
        var ordersForProduct = _orderRepository.GetByProductId(productId);
        if (!ordersForProduct.Any())
            return;

        var shippedOrders = ordersForProduct.Count(x => x.Status == OrderStatus.Shipped);
        var cancelledOrders = ordersForProduct.Count(x => x.Status == OrderStatus.Cancelled);
        var ordersInProgress = ordersForProduct.Count() - shippedOrders - cancelledOrders;

        _eventBus.Publish(new ProductOrdersStatusReportChangedIntegrationEvent(
            productId, (uint)ordersInProgress, (uint)shippedOrders, (uint)cancelledOrders));
    }
}

namespace Bazaar.Ordering.Application.EventHandling;

public class OrderPaymentSucceededIntegrationEventHandler
    : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly ILogger<OrderPaymentSucceededIntegrationEventHandler> _logger;

    public OrderPaymentSucceededIntegrationEventHandler(
        IOrderRepository orderRepo,
        ILogger<OrderPaymentSucceededIntegrationEventHandler> logger)
    {
        _orderRepo = orderRepo;
        _logger = logger;
    }

    public async Task Handle(OrderPaymentSucceededIntegrationEvent @event)
    {
        var order = _orderRepo.GetById(@event.orderId);
        if (order == null)
            return;

        _logger.LogInformation("Payment success, now waiting for seller's confirmation.");

        order.RequestSellerConfirmation();
        _orderRepo.Update(order);
        await Task.CompletedTask;
    }
}
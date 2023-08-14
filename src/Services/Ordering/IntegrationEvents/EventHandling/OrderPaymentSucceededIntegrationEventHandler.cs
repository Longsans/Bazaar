namespace Bazaar.Ordering.IntegrationEvents.EventHandling;

public class OrderPaymentSucceededIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
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
        _logger.LogInformation("Payment success, now waiting for seller's confirmation.");
        _orderRepo.UpdateStatus(@event.orderId, OrderStatus.AwaitingSellerConfirmation);
        await Task.CompletedTask;
    }
}
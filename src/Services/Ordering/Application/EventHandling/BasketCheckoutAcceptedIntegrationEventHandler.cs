namespace Bazaar.Ordering.Application.EventHandling;

public class BasketCheckoutAcceptedIntegrationEventHandler
    : IIntegrationEventHandler<BasketCheckoutAcceptedIntegrationEvent>
{
    private readonly HandleOrderService _handleOrderService;

    public BasketCheckoutAcceptedIntegrationEventHandler(HandleOrderService handleOrderService)
    {
        _handleOrderService = handleOrderService;
    }

    public async Task Handle(BasketCheckoutAcceptedIntegrationEvent @event)
    {
        if (!@event.BasketItems.Any())
            return;

        var items = @event.BasketItems.Select(x => new OrderItem(
            x.ProductId, x.ProductName, x.ProductUnitPrice, x.Quantity, default));

        var order = new Order(
            string.Join(", ", @event.ShippingAddress,
                @event.City, @event.Country),
            @event.BuyerId, items);

        _handleOrderService.PlaceOrder(order);
        await Task.CompletedTask;
    }
}

namespace Bazaar.Ordering.ServiceIntegration.EventHandling;

public class BuyerCheckoutAcceptedIntegrationEventHandler : IIntegrationEventHandler<BuyerCheckoutAcceptedIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly IEventBus _eventBus;

    public BuyerCheckoutAcceptedIntegrationEventHandler(IOrderRepository orderRepo, IEventBus eventBus)
    {
        _orderRepo = orderRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(BuyerCheckoutAcceptedIntegrationEvent @event)
    {
        if (!@event.Basket.Items.Any())
            return;

        var items = @event.Basket.Items.Select(x => new OrderItem(
            x.ProductId, x.ProductName, x.ProductUnitPrice, x.Quantity, default));

        var order = new Order(
            string.Join(", ", @event.ShippingAddress,
                @event.City, @event.Country),
            @event.BuyerId, items);

        _orderRepo.Create(order);
        _eventBus.Publish(
                new OrderCreatedIntegrationEvent(order.Id, order.Items.Select(
                    item => new OrderStockItem(item.ProductId, item.Quantity))));

        await Task.CompletedTask;
    }
}

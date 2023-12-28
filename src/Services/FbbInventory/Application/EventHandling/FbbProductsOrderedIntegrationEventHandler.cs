namespace Bazaar.FbbInventory.Application.EventHandling;

public class FbbProductsOrderedIntegrationEventHandler
    : IIntegrationEventHandler<FbbProductsOrderedIntegrationEvent>
{
    private readonly StockTransactionService _stockTxnService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<FbbProductsOrderedIntegrationEventHandler> _logger;

    public FbbProductsOrderedIntegrationEventHandler(
        StockTransactionService stockTxnService,
        IEventBus eventBus,
        ILogger<FbbProductsOrderedIntegrationEventHandler> logger)
    {
        _stockTxnService = stockTxnService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(FbbProductsOrderedIntegrationEvent @event)
    {
        if (!@event.OrderedProducts.Any())
        {
            _logger.LogError("Error processing ordered FBB products for order {orderId}: " +
                "Ordered products list empty.", @event.OrderId);
            return;
        }

        var issueQuantities = @event.OrderedProducts.Select(x =>
            new OutboundStockQuantity(x.ProductId, x.OrderedQuantity, 0u));
        var result = await _stockTxnService.IssueStocksFifo(issueQuantities, StockIssueReason.Sale);
        if (result.IsSuccess)
        {
            var issuedProductIds = result.Value.Items.Select(x => x.ProductId);
            _eventBus.Publish(new OrderItemsStockConfirmedIntegrationEvent(
                @event.OrderId, issuedProductIds));
            _logger.LogCritical("Success processing ordered FBB products for order {orderId}. " +
                "Stocks have been issued.", @event.OrderId);
        }
        else
        {
            var rejectedItems = issueQuantities.Select(x => new StockRejectedOrderItem(
                    x.ProductId, x.GoodQuantity, 0, StockRejectionReason.InsufficientStock));
            _eventBus.Publish(new OrderItemsStockRejectedIntegrationEvent(@event.OrderId, rejectedItems));
        }

        await Task.CompletedTask;
    }
}

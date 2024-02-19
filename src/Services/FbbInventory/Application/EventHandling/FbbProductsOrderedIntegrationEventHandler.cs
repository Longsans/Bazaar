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
        var stockCheckResult = await _stockTxnService.CheckStockForIssuance(issueQuantities);
        if (!stockCheckResult.IsSuccess)
        {
            _logger.LogError("Error checking stock for issuance, FBB product created event {eventId}: {errors}.", @event.Id, stockCheckResult.GetJoinedErrorMessage());
            return;
        }
        if (!stockCheckResult.Value.CanSatisfyDemand)
        {
            var items = stockCheckResult.Value.Items.Where(x => !x.CanSatisfyDemand).Select(x =>
            {
                var rejectReason = !x.IsListed ? StockRejectionReason.NoListing : StockRejectionReason.InsufficientStock;
                return new StockRejectedOrderItem(x.ProductId, x.DemandedGoodUnits, x.GoodStockCount, rejectReason);
            });
            _eventBus.Publish(new OrderItemsStockRejectedIntegrationEvent(@event.OrderId, items));
            _logger.LogCritical("Stock check complete for order {orderId}: Stock rejected for products {productIds}.",
                @event.OrderId, string.Join(", ", items.Select(x => x.ProductId)));
            return;
        }

        var result = await _stockTxnService.IssueStocksFifo(issueQuantities, StockIssueReason.Sale);
        if (!result.IsSuccess)
        {
            _logger.LogError("Error issuing stock for order {orderId}, FBB products created event {eventId}: {errors}.",
                @event.OrderId, @event.Id, result.GetJoinedErrorMessage());
        }
        var issuedProductIds = result.Value.Items.Select(x => x.ProductId);
        _eventBus.Publish(new OrderItemsStockConfirmedIntegrationEvent(@event.OrderId, issuedProductIds));
        _logger.LogCritical("Success processing ordered FBB products for order {orderId}. " +
            "Stocks have been issued.", @event.OrderId);
    }
}

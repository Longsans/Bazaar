namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class ProductListingClosedIntegrationEventHandler
    : IIntegrationEventHandler<ProductListingClosedIntegrationEvent>
{
    private readonly StockAdjustmentService _stockAdjustmentService;
    private readonly ILogger<ProductListingClosedIntegrationEventHandler> _logger;

    public ProductListingClosedIntegrationEventHandler(
        StockAdjustmentService stockAdjustmentService,
        ILogger<ProductListingClosedIntegrationEventHandler> logger)
    {
        _stockAdjustmentService = stockAdjustmentService;
        _logger = logger;
    }

    public async Task Handle(ProductListingClosedIntegrationEvent @event)
    {
        var result = _stockAdjustmentService.RenderProductStockStranded(@event.ProductId);
        if (!result.IsSuccess)
        {
            _logger.LogError("Error rendering stock stranded for product {ProductId}: {ErrorMessage}",
                @event.ProductId, result.GetJoinedErrorMessage());
        }
        await Task.CompletedTask;
    }
}

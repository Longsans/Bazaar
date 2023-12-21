namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class ProductRelistedIntegrationEventHandler :
    IIntegrationEventHandler<ProductRelistedIntegrationEvent>
{
    private readonly StockAdjustmentService _stockAdjustmentService;
    private readonly ILogger<ProductRelistedIntegrationEventHandler> _logger;

    public ProductRelistedIntegrationEventHandler(
        StockAdjustmentService stockAdjustmentService,
        ILogger<ProductRelistedIntegrationEventHandler> logger)
    {
        _stockAdjustmentService = stockAdjustmentService;
        _logger = logger;
    }

    public async Task Handle(ProductRelistedIntegrationEvent @event)
    {
        var result = _stockAdjustmentService.ConfirmStockStrandingResolved(@event.ProductId);
        if (!result.IsSuccess)
        {
            _logger.LogError("Error confirming stock stranding resolved for product {ProductId}: {ErrorMessage}",
                @event.ProductId, result.GetJoinedErrorMessage());
        }
        await Task.CompletedTask;
    }
}

namespace Bazaar.FbbInventory.ServiceIntegration.EventHandling;

public class ProductFulfillmentChangedToMerchantIntegrationEventHandler
    : IIntegrationEventHandler<ProductFulfillmentChangedToMerchantIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly StockAdjustmentService _stockAdjustmentService;
    private readonly ILogger<ProductFulfillmentChangedToMerchantIntegrationEventHandler> _logger;
    private readonly IEventBus _eventBus;

    public ProductFulfillmentChangedToMerchantIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo, StockAdjustmentService stockAdjustmentService,
        IEventBus eventBus, ILogger<ProductFulfillmentChangedToMerchantIntegrationEventHandler> logger)
    {
        _productInventoryRepo = productInventoryRepo;
        _stockAdjustmentService = stockAdjustmentService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(ProductFulfillmentChangedToMerchantIntegrationEvent @event)
    {
        var inventory = _productInventoryRepo.GetByProductId(@event.ProductId);
        if (inventory == null)
        {
            _eventBus.Publish(new ProductFbbInventoryDeletedIntegrationEvent(@event.ProductId));
            return;
        }

        var result = _stockAdjustmentService.RenderProductStockStranded(@event.ProductId);
        if (!result.IsSuccess)
        {
            _logger.LogError("Error rendering stock stranded for product {ProductId}: {ErrorMessage}",
                @event.ProductId, result.GetJoinedErrorMessage());
        }
        await Task.CompletedTask;
    }
}

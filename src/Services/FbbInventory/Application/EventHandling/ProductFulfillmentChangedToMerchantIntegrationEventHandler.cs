namespace Bazaar.FbbInventory.Application.EventHandling;

public class ProductFulfillmentChangedToMerchantIntegrationEventHandler
    : IIntegrationEventHandler<ProductFulfillmentChangedToMerchantIntegrationEvent>
{
    private readonly IRepository<ProductInventory> _productInventoryRepo;
    private readonly StockAdjustmentService _stockAdjustmentService;
    private readonly ILogger<ProductFulfillmentChangedToMerchantIntegrationEventHandler> _logger;
    private readonly IEventBus _eventBus;

    public ProductFulfillmentChangedToMerchantIntegrationEventHandler(
        IRepository<ProductInventory> productInventoryRepo, StockAdjustmentService stockAdjustmentService,
        IEventBus eventBus, ILogger<ProductFulfillmentChangedToMerchantIntegrationEventHandler> logger)
    {
        _productInventoryRepo = productInventoryRepo;
        _stockAdjustmentService = stockAdjustmentService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(ProductFulfillmentChangedToMerchantIntegrationEvent @event)
    {
        var inventory = await _productInventoryRepo.SingleOrDefaultAsync(
            new ProductInventoryWithLotsAndSellerSpec(@event.ProductId));
        if (inventory == null)
        {
            _eventBus.Publish(new ProductFbbInventoryDeletedIntegrationEvent(@event.ProductId));
            return;
        }

        var result = await _stockAdjustmentService.RenderProductStockStranded(@event.ProductId);
        if (!result.IsSuccess)
        {
            _logger.LogError("Error rendering stock stranded for product {ProductId}: {ErrorMessage}",
                @event.ProductId, result.GetJoinedErrorMessage());
        }
        await Task.CompletedTask;
    }
}

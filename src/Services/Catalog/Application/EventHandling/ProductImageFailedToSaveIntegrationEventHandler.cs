namespace Bazaar.Catalog.Application.EventHandling;

public class ProductImageFailedToSaveIntegrationEventHandler : IIntegrationEventHandler<ProductImageFailedToSaveIntegrationEvent>
{
    private readonly ILogger<ProductImageFailedToSaveIntegrationEventHandler> _logger;

    public ProductImageFailedToSaveIntegrationEventHandler(ILogger<ProductImageFailedToSaveIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(ProductImageFailedToSaveIntegrationEvent @event)
    {
        _logger.LogCritical("Saving image failed for product {productId}: {errorMessage}", @event.ProductId, @event.FailureMessage);
        await Task.CompletedTask;
    }
}

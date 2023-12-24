namespace Bazaar.FbbInventory.Application.EventHandling;

public class CatalogItemDeletedIntegrationEventHandler
    : IIntegrationEventHandler<CatalogItemDeletedIntegrationEvent>
{
    private readonly IDeleteProductInventoryService _deleteProductInventoryService;

    public CatalogItemDeletedIntegrationEventHandler(
        IDeleteProductInventoryService deleteService)
    {
        _deleteProductInventoryService = deleteService;
    }

    public async Task Handle(CatalogItemDeletedIntegrationEvent @event)
    {
        _deleteProductInventoryService
            .DeleteProductInventory(@event.ProductId);
        await Task.CompletedTask;
    }
}

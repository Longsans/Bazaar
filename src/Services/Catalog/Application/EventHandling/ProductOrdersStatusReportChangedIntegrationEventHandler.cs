namespace Bazaar.Catalog.Application.EventHandling;

public class ProductOrdersStatusReportChangedIntegrationEventHandler
    : IIntegrationEventHandler<ProductOrdersStatusReportChangedIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;

    public ProductOrdersStatusReportChangedIntegrationEventHandler(
        ICatalogRepository catalogRepo)
    {
        _catalogRepo = catalogRepo;
    }

    public async Task Handle(ProductOrdersStatusReportChangedIntegrationEvent @event)
    {
        // Catalog item cannot be null here since it's not possible to
        // place or fulfill an order for a product that's been deleted and vice versa
        var catalogItem = _catalogRepo.GetByProductId(@event.ProductId)!;
        catalogItem.UpdateHasOrdersInProgressStatus(@event.OrdersInProgress != 0);
        _catalogRepo.Update(catalogItem);

        await Task.CompletedTask;
    }
}

namespace Bazaar.Catalog.Application.EventHandling;

public class ProductOrdersStatusReportChangedIntegrationEventHandler
    : IIntegrationEventHandler<ProductOrdersStatusReportChangedIntegrationEvent>
{
    private readonly IRepository<CatalogItem> _catalogRepo;

    public ProductOrdersStatusReportChangedIntegrationEventHandler(
        IRepository<CatalogItem> catalogRepo)
    {
        _catalogRepo = catalogRepo;
    }

    public async Task Handle(ProductOrdersStatusReportChangedIntegrationEvent @event)
    {
        // Catalog item cannot be null here since it's not possible to
        // place or fulfill an order for a product that's been deleted and vice versa
        var catalogItem = await _catalogRepo.SingleOrDefaultAsync(
            new CatalogItemByProductIdSpec(@event.ProductId));
        catalogItem!.UpdateHasOrdersInProgress(@event.OrdersInProgress != 0);
        await _catalogRepo.UpdateAsync(catalogItem);

        await Task.CompletedTask;
    }
}

namespace Bazaar.Catalog.Application.EventHandling;

public class ClientAccountClosedIntegrationEventHandler
    : IIntegrationEventHandler<ClientAccountClosedIntegrationEvent>
{
    private readonly IRepository<CatalogItem> _catalogRepo;
    private readonly ListingService _listingService;
    private readonly IEventBus _eventBus;

    public ClientAccountClosedIntegrationEventHandler(
        IRepository<CatalogItem> catalogRepo,
        ListingService listingService,
        IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _listingService = listingService;
        _eventBus = eventBus;
    }

    public async Task Handle(ClientAccountClosedIntegrationEvent @event)
    {
        var products = await _catalogRepo.ListAsync(new CatalogItemsBySellerIdSpec(@event.ClientId));
        var undeletableProducts = products.Where(x => !x.IsDeletable);
        if (undeletableProducts.Any())
        {
            var failedDeletes = undeletableProducts.Select(x =>
                new FailedDeleteListing(x.ProductId, x.HasOrdersInProgress
                ? ListingDeleteFailureReason.HasOrdersInProgress
                : ListingDeleteFailureReason.HasFbbStock));
            _eventBus.Publish(new ProductListingsDeleteFailedIntegrationEvent(
                failedDeletes, @event.ClientId));
            return;
        }
        foreach (var product in products)
        {
            await _listingService.DeleteListing(product.ProductId);
        }
    }
}

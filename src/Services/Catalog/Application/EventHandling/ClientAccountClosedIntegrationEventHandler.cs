namespace Bazaar.Catalog.Application.EventHandling;

public class ClientAccountClosedIntegrationEventHandler
    : IIntegrationEventHandler<ClientAccountClosedIntegrationEvent>
{
    private readonly IRepository<CatalogItem> _catalogRepo;
    private readonly DeleteCatalogItemService _deleteCatalogItemService;
    private readonly IEventBus _eventBus;

    public ClientAccountClosedIntegrationEventHandler(
        IRepository<CatalogItem> catalogRepo,
        DeleteCatalogItemService deleteCatalogItemService,
        IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _deleteCatalogItemService = deleteCatalogItemService;
        _eventBus = eventBus;
    }

    public async Task Handle(ClientAccountClosedIntegrationEvent @event)
    {
        var products = await _catalogRepo.ListAsync(new CatalogItemsBySellerIdSpec(@event.ClientId));
        var productsWithOrdersInProgress = new List<string>();
        var productsWithFbbStocks = new List<string>();
        foreach (var product in products)
        {
            try
            {
                _deleteCatalogItemService.AssertCanBeDeleted(product);
            }
            catch (DeleteProductWithOrdersInProgressException)
            {
                productsWithOrdersInProgress.Add(product.ProductId);
            }
            catch (DeleteFbbProductWhenFbbInventoryNotEmptyException)
            {
                productsWithFbbStocks.Add(product.ProductId);
            }
        }

        if (productsWithOrdersInProgress.Any())
        {
            _eventBus.Publish(new ProductsHaveOrdersInProgressIntegrationEvent(
                productsWithOrdersInProgress, @event.ClientId));
        }
        else if (productsWithFbbStocks.Any())
        {
            _eventBus.Publish(new ProductsHaveFbbStocksIntegrationEvent(
                productsWithFbbStocks, @event.ClientId));
        }
        else
        {
            foreach (var product in products)
            {
                await _deleteCatalogItemService.SoftDeleteById(product.Id);
            }
        }

        await Task.CompletedTask;
    }
}

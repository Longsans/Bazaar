namespace Bazaar.Catalog.ServiceIntegration.EventHandling;

public class ClientAccountClosedIntegrationEventHandler
    : IIntegrationEventHandler<ClientAccountClosedIntegrationEvent>
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IDeleteCatalogItemService _deleteCatalogItemService;
    private readonly IEventBus _eventBus;

    public ClientAccountClosedIntegrationEventHandler(
        ICatalogRepository catalogRepo,
        IDeleteCatalogItemService deleteCatalogItemService,
        IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _deleteCatalogItemService = deleteCatalogItemService;
        _eventBus = eventBus;
    }

    public async Task Handle(ClientAccountClosedIntegrationEvent @event)
    {
        var products = _catalogRepo.GetBySellerId(@event.ClientId).ToList();
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
                _deleteCatalogItemService.SoftDeleteById(product.Id);
            }
        }

        await Task.CompletedTask;
    }
}

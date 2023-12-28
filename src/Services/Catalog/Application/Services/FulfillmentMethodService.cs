namespace Bazaar.Catalog.Application.Services;

public class FulfillmentMethodService
{
    private readonly IRepositoryBase<CatalogItem> _catalogRepo;
    private readonly IEventBus _eventBus;

    public FulfillmentMethodService(
        IRepositoryBase<CatalogItem> catalogRepo, IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public async Task<Result> ChangeFulfillmentMethodToFbb(string productId)
    {
        var catalogItem = await _catalogRepo.SingleOrDefaultAsync(
            new CatalogItemByProductIdSpec(productId));
        if (catalogItem == null)
        {
            return Result.NotFound();
        }

        try
        {
            catalogItem.ChangeFulfillmentMethodToFbb();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }
        await _catalogRepo.UpdateAsync(catalogItem);
        _eventBus.Publish(
            new ProductFulfillmentMethodChangedToFbbIntegrationEvent(productId));
        return Result.Success();
    }

    public async Task<Result> ChangeFulfillmentMethodToMerchant(string productId)
    {
        var catalogItem = await _catalogRepo.SingleOrDefaultAsync(
            new CatalogItemByProductIdSpec(productId));
        if (catalogItem == null)
        {
            return Result.NotFound();
        }

        try
        {
            catalogItem.ChangeFulfillmentMethodToMerchant();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Conflict(ex.Message);
        }
        await _catalogRepo.UpdateAsync(catalogItem);
        _eventBus.Publish(
            new ProductFulfillmentMethodChangedToMerchantIntegrationEvent(productId));
        return Result.Success();
    }
}

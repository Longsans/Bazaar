namespace Bazaar.Catalog.Domain.Services;

public class FulfillmentMethodService : IFulfillmentMethodService
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IEventBus _eventBus;

    public FulfillmentMethodService(
        ICatalogRepository catalogRepo, IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public Result ChangeFulfillmentMethodToFbb(string productId)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
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
        _catalogRepo.Update(catalogItem);
        _eventBus.Publish(
            new ProductFulfillmentMethodChangedToFbbIntegrationEvent(productId));
        return Result.Success();
    }

    public Result ChangeFulfillmentMethodToMerchant(string productId)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
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
        _catalogRepo.Update(catalogItem);
        _eventBus.Publish(
            new ProductFulfillmentMethodChangedToMerchantIntegrationEvent(productId));
        return Result.Success();
    }
}

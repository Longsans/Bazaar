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

    public Result ChangeToFulfillmentByBazaar(string productId)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null)
        {
            return Result.NotFound();
        }

        if (!catalogItem.IsFulfilledByBazaar)
        {
            catalogItem.UpdateFulfillmentByBazaar(true);
            catalogItem.UpdateOfficiallyListed(false);
            _catalogRepo.Update(catalogItem);
            _eventBus.Publish(
                new ProductFulfillmentChangedToFbbIntegrationEvent(productId));
        }
        return Result.Success();
    }

    public Result ChangeToFulfillmentByMerchant(string productId)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null)
        {
            return Result.NotFound();
        }

        if (catalogItem.IsFulfilledByBazaar)
        {
            catalogItem.UpdateFulfillmentByBazaar(false);
            catalogItem.UpdateOfficiallyListed(false);
            _catalogRepo.Update(catalogItem);
            _eventBus.Publish(
                new ProductFulfillmentChangedToMerchantIntegrationEvent(productId));
        }
        return Result.Success();
    }
}

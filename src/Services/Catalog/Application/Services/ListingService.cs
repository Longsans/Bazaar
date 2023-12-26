namespace Bazaar.Catalog.Application.Services;

public class ListingService
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IEventBus _eventBus;

    public ListingService(ICatalogRepository catalogRepo, IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public Result CloseListing(string productId)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null)
        {
            return Result.NotFound();
        }

        try
        {
            catalogItem.CloseListing();
        }
        catch (InvalidOperationException)
        {
            return Result.Conflict("Listing deleted.");
        }

        _catalogRepo.Update(catalogItem);
        _eventBus.Publish(new ProductListingClosedIntegrationEvent(catalogItem.ProductId));
        return Result.Success();
    }

    public Result Relist(string productId)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null)
        {
            return Result.NotFound();
        }

        try
        {
            catalogItem.Relist();
        }
        catch (InvalidOperationException)
        {
            return Result.Conflict("Relisting is only available for listings in Inactive (Closed) status.");
        }

        _catalogRepo.Update(catalogItem);
        _eventBus.Publish(new ProductRelistedIntegrationEvent(catalogItem.ProductId));
        return Result.Success();
    }
}

﻿namespace Bazaar.Catalog.Application.Services;

public class ListingService
{
    private readonly IRepository<CatalogItem> _catalogRepo;
    private readonly IEventBus _eventBus;

    public ListingService(IRepository<CatalogItem> catalogRepo, IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public async Task<Result> CloseListing(string productId)
    {
        var catalogItem = await _catalogRepo.SingleOrDefaultAsync(
            new CatalogItemByProductIdSpec(productId));
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

        await _catalogRepo.UpdateAsync(catalogItem);
        _eventBus.Publish(new ProductListingClosedIntegrationEvent(catalogItem.ProductId));
        return Result.Success();
    }

    public async Task<Result> Relist(string productId)
    {
        var catalogItem = await _catalogRepo.SingleOrDefaultAsync(
            new CatalogItemByProductIdSpec(productId));
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

        await _catalogRepo.UpdateAsync(catalogItem);
        _eventBus.Publish(new ProductRelistedIntegrationEvent(catalogItem.ProductId));
        return Result.Success();
    }

    public async Task<Result> DeleteListing(string productId)
    {
        var item = await _catalogRepo.SingleOrDefaultAsync(
            new CatalogItemByProductIdSpec(productId));
        if (item == null)
        {
            return Result.NotFound("Catalog item not found.");
        }

        if (item.IsDeleted)
        {
            return Result.Success();
        }

        try
        {
            item.DeleteListing();
        }
        catch (Exception ex)
        {
            return Result.Conflict(ex.Message);
        }
        await _catalogRepo.UpdateAsync(item);
        _eventBus.Publish(
            new ProductListingDeletedIntegrationEvent(item.ProductId));

        return Result.Success();
    }
}

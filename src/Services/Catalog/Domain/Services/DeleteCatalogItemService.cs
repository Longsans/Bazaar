namespace Bazaar.Catalog.Domain.Services;

public class DeleteCatalogItemService : IDeleteCatalogItemService
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IEventBus _eventBus;

    public DeleteCatalogItemService(
        ICatalogRepository catalogRepo,
        IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public Result SoftDeleteById(int id)
    {
        var catalogItem = _catalogRepo.GetById(id);
        if (catalogItem == null)
        {
            return Result.NotFound("Catalog item not found.");
        }

        return TrySoftDelete(catalogItem);
    }

    public Result SoftDeleteByProductId(string productId)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null)
        {
            return Result.NotFound("Catalog item not found.");
        }

        return TrySoftDelete(catalogItem);
    }

    private Result TrySoftDelete(CatalogItem item)
    {
        if (item.IsDeleted)
        {
            return Result.Success();
        }

        // Each order must have been either shipped or cancelled for us to be able to delete the product
        if (!item.HasNoOrdersInProgress)
        {
            return Result.Conflict(
                "This product cannot be deleted while it has orders in progress.");
        }

        if (item.IsFulfilledByBazzar && item.AvailableStock > 0)
        {
            return Result.Conflict(
                "Cannot delete an FBB product when FBB inventory has not been emptied.");
        }

        item.Delete();
        _catalogRepo.Update(item);
        _eventBus.Publish(
            new CatalogItemDeletedIntegrationEvent(item.ProductId));

        return Result.Success();
    }
}

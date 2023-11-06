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

    public void AssertCanBeDeleted(CatalogItem item)
    {
        // Each order must have been either shipped or cancelled for us to be able to delete the product
        if (item.HasOrdersInProgress)
        {
            throw new DeleteProductWithOrdersInProgressException();
        }

        // If the product is fulfilled by Bazaar then all FBB stock must be moved out before deleting the product
        if (item.IsFulfilledByBazaar && item.AvailableStock > 0)
        {
            throw new DeleteFbbProductWhenFbbInventoryNotEmptyException();
        }
    }

    public void SoftDeleteById(int id)
    {
        var catalogItem = _catalogRepo.GetById(id);
        if (catalogItem == null)
        {
            return;
        }

        TrySoftDelete(catalogItem);
    }

    public void SoftDeleteByProductId(string productId)
    {
        var catalogItem = _catalogRepo.GetByProductId(productId);
        if (catalogItem == null)
        {
            return;
        }

        TrySoftDelete(catalogItem);
    }

    private void TrySoftDelete(CatalogItem item)
    {
        if (item.IsDeleted)
        {
            return;
        }

        AssertCanBeDeleted(item);

        item.ReduceStock(item.AvailableStock);
        item.Delete();
        _catalogRepo.Update(item);
        _eventBus.Publish(
            new CatalogItemDeletedIntegrationEvent(item.ProductId));

        return;
    }
}

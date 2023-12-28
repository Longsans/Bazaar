namespace Bazaar.Catalog.Application.Services;

public class DeleteCatalogItemService
{
    private readonly IRepositoryBase<CatalogItem> _catalogRepo;
    private readonly IEventBus _eventBus;

    public DeleteCatalogItemService(
        IRepositoryBase<CatalogItem> catalogRepo,
        IEventBus eventBus)
    {
        _catalogRepo = catalogRepo;
        _eventBus = eventBus;
    }

    public void AssertCanBeDeleted(CatalogItem item)
    {
        // All orders must be completed in order to delete product
        if (item.HasOrdersInProgress)
        {
            throw new DeleteProductWithOrdersInProgressException();
        }

        // If the product is fulfilled by Bazaar then all FBB stock must be moved out before deleting
        if (item.IsFbb && item.AvailableStock > 0)
        {
            throw new DeleteFbbProductWhenFbbInventoryNotEmptyException();
        }
    }

    public async Task SoftDeleteById(int id)
    {
        var catalogItem = await _catalogRepo.GetByIdAsync(id);
        if (catalogItem == null)
        {
            return;
        }

        await TrySoftDelete(catalogItem);
    }

    public async Task SoftDeleteByProductId(string productId)
    {
        var catalogItem = await _catalogRepo.SingleOrDefaultAsync(
            new CatalogItemByProductIdSpec(productId));
        if (catalogItem == null)
        {
            return;
        }

        await TrySoftDelete(catalogItem);
    }

    private async Task TrySoftDelete(CatalogItem item)
    {
        if (item.IsDeleted)
        {
            return;
        }

        AssertCanBeDeleted(item);

        item.ReduceStock(item.AvailableStock);
        item.Delete();
        await _catalogRepo.UpdateAsync(item);
        _eventBus.Publish(
            new CatalogItemDeletedIntegrationEvent(item.ProductId));

        return;
    }
}

namespace Bazaar.FbbInventory.Application.Services;

public class DeleteProductInventoryService
{
    private readonly IRepository<ProductInventory> _productInventoryRepo;
    private readonly IEventBus _eventBus;

    public DeleteProductInventoryService(
        IRepository<ProductInventory> productInventoryRepo, IEventBus eventBus)
    {
        _productInventoryRepo = productInventoryRepo;
        _eventBus = eventBus;
    }

    public async Task<Result> DeleteProductInventory(int id)
    {
        return await DeleteAndPublishEvent(
            new ProductInventoryWithLotsAndSellerSpec(id));
    }

    public async Task<Result> DeleteProductInventory(string productId)
    {
        return await DeleteAndPublishEvent(
            new ProductInventoryWithLotsAndSellerSpec(productId));
    }

    private async Task<Result> DeleteAndPublishEvent(ISingleResultSpecification<ProductInventory> spec)
    {
        var productInventory = await _productInventoryRepo.SingleOrDefaultAsync(spec);
        if (productInventory == null)
        {
            return Result.Success();
        }
        if (productInventory.TotalUnits > 0)
        {
            return Result.Conflict(
                "Cannot delete product inventory while it still has units being used in operations.");
        }
        if (productInventory.HasPickupsInProgress)
        {
            return Result.Conflict(
                "Cannot delete product inventory while there are pickups in progress for it.");
        }

        productInventory.SellerInventory.ProductInventories.Remove(productInventory);
        _eventBus.Publish(
            new ProductFbbInventoryDeletedIntegrationEvent(productInventory.ProductId));

        return Result.Success();
    }
}

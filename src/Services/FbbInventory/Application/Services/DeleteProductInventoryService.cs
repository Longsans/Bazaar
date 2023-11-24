namespace Bazaar.FbbInventory.Application.Services;

public class DeleteProductInventoryService : IDeleteProductInventoryService
{
    private readonly ISellerInventoryRepository _sellerInventoryRepo;
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly IEventBus _eventBus;

    public DeleteProductInventoryService(
        ISellerInventoryRepository sellerInventoryRepo,
        IProductInventoryRepository productInventoryRepo,
        IEventBus eventBus)
    {
        _sellerInventoryRepo = sellerInventoryRepo;
        _productInventoryRepo = productInventoryRepo;
        _eventBus = eventBus;
    }

    public Result DeleteProductInventory(int id)
    {
        var productInventory = _productInventoryRepo.GetById(id);
        if (productInventory == null)
        {
            return Result.Success();
        }
        return DeleteAndPublishEvent(productInventory);
    }

    public Result DeleteProductInventory(string productId)
    {
        var productInventory = _productInventoryRepo.GetByProductId(productId);
        if (productInventory == null)
        {
            return Result.Success();
        }
        return DeleteAndPublishEvent(productInventory);
    }

    private Result DeleteAndPublishEvent(ProductInventory productInventory)
    {
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

        var sellerInventory = _sellerInventoryRepo.GetWithProductsById(
            productInventory.SellerInventoryId)!;
        sellerInventory.ProductInventories.Remove(productInventory);
        _sellerInventoryRepo.Update(sellerInventory);
        _eventBus.Publish(
            new ProductFbbInventoryDeletedIntegrationEvent(productInventory.ProductId));

        return Result.Success();
    }
}

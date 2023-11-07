using Bazaar.Inventory.Domain.Interfaces;

namespace Bazaar.Inventory.ServiceIntegration.EventHandling;

public class CatalogItemDeletedIntegrationEventHandler
    : IIntegrationEventHandler<CatalogItemDeletedIntegrationEvent>
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly ISellerInventoryRepository _sellerInventoryRepo;

    public CatalogItemDeletedIntegrationEventHandler(
        IProductInventoryRepository productInventoryRepo,
        ISellerInventoryRepository sellerInventoryRepo)
    {
        _productInventoryRepo = productInventoryRepo;
        _sellerInventoryRepo = sellerInventoryRepo;
    }

    public async Task Handle(CatalogItemDeletedIntegrationEvent @event)
    {
        var productInventory = _productInventoryRepo
            .GetByProductId(@event.ProductId);
        if (productInventory == null)
        {
            return;
        }

        var sellerInventory = _sellerInventoryRepo
            .GetWithProductsById(productInventory.SellerInventoryId)!;

        sellerInventory.ProductInventories.Remove(productInventory);
        _sellerInventoryRepo.Update(sellerInventory);
        await Task.CompletedTask;
    }
}

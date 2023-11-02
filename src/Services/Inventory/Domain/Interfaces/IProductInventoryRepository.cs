namespace Bazaar.Inventory.Domain.Interfaces;

public interface IProductInventoryRepository
{
    ProductInventory? GetByProductId(string productId);
    void Update(ProductInventory productInventory);
}

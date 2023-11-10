namespace Bazaar.Inventory.Domain.Interfaces;

public interface IProductInventoryRepository
{
    ProductInventory? GetById(int id);
    ProductInventory? GetByProductId(string productId);
    void Update(ProductInventory productInventory);
}

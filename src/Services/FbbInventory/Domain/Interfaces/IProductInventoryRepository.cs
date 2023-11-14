namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IProductInventoryRepository
{
    IEnumerable<ProductInventory> GetAll();
    ProductInventory? GetById(int id);
    ProductInventory? GetByProductId(string productId);
    void Update(ProductInventory productInventory);
    void Delete(ProductInventory productInventory);
    void DeleteRange(IEnumerable<ProductInventory> productInventories);
}

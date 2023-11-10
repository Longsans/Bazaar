namespace Bazaar.Inventory.Domain.Interfaces;

public interface IDeleteProductInventoryService
{
    Result DeleteProductInventory(int id);
    Result DeleteProductInventory(string productId);
}

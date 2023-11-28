namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IDeleteProductInventoryService
{
    Result DeleteProductInventory(int id);
    Result DeleteProductInventory(string productId);
}

namespace Bazaar.Transport.Domain.Interfaces;

public interface IInventoryReturnRepository
{
    InventoryReturn? GetById(int id);
    IEnumerable<InventoryReturn> GetByInventoryOwnerId(string ownerId);
    IEnumerable<InventoryReturn> GetIncomplete();
    InventoryReturn Create(InventoryReturn inventoryReturn);
    void Update(InventoryReturn inventoryReturn);
}

namespace Bazaar.Transport.Domain.Interfaces;

public interface IInventoryPickupRepository
{
    InventoryPickup? GetById(int id);
    IEnumerable<InventoryPickup> GetIncomplete();
    InventoryPickup Create(InventoryPickup inventoryPickup);
    void Update(InventoryPickup inventoryPickup);
}

namespace Bazaar.Transport.Domain.Interfaces;

public interface IInventoryPickupRepository
{
    InventoryPickup? GetById(int id);
    IEnumerable<InventoryPickup> GetByProductId(string productId);
    IEnumerable<InventoryPickup> GetIncomplete();
    IEnumerable<InventoryPickup> GetScheduled(string productId);
    IEnumerable<InventoryPickup> GetInProgress(string productId);
    IEnumerable<InventoryPickup> GetCompleted(string productId);
    IEnumerable<InventoryPickup> GetCancelled(string productId);
    IEnumerable<InventoryPickup> GetAllCancelled();
    InventoryPickup Create(InventoryPickup inventoryPickup);
    void Update(InventoryPickup inventoryPickup);
    void Delete(InventoryPickup inventoryPickup);
}

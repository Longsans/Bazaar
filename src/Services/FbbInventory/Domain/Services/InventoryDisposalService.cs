namespace Bazaar.FbbInventory.Domain.Services;

public class InventoryDisposalService : IInventoryDisposalService
{
    private readonly IProductInventoryRepository _productInventoryRepo;

    public InventoryDisposalService(IProductInventoryRepository productInventoryRepo)
    {
        _productInventoryRepo = productInventoryRepo;
    }

    public void MarkOverdueUnfulfillableInventoriesForDisposal()
    {
        var overdueInventories = _productInventoryRepo.GetAll()
            .Where(x => x.UnfulfillableSince + StoragePolicy.MaximumUnfulfillableDuration <= DateTime.Now.Date)
            .ToList();

        foreach (var inventory in overdueInventories)
        {
            inventory.MarkToBeDisposed();
            _productInventoryRepo.Update(inventory);
        }
    }

    public void DisposeMarkedInventories()
    {
        var toBeDisposedInventories = _productInventoryRepo.GetAll()
            .Where(x => x.Status == InventoryStatus.ToBeDisposed)
            .ToList();

        _productInventoryRepo.DeleteRange(toBeDisposedInventories);
    }
}

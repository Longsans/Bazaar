namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IInventoryDisposalService
{
    void MarkOverdueUnfulfillableInventoriesForDisposal();
    void DisposeMarkedInventories();
}

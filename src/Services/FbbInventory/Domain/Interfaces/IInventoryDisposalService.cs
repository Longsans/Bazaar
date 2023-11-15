namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IInventoryDisposalService
{
    void MarkOverdueUnfulfillableInventoryForDisposal();
    void DisposeMarkedInventories();
}

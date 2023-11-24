namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IRemovalService
{
    Result RequestRemovalForStockUnits(
        IEnumerable<StockUnitsRemovalDto> removalRequests, RemovalMethod removalMethod);
    Result RequestRemovalForLots(IEnumerable<string> lotNumbers, RemovalMethod removalMethod);
    void RequestDisposalForLotsUnfulfillableBeyondPolicyDuration();
}

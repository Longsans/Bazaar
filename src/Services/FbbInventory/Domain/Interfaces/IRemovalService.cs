namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IRemovalService
{
    Result RequestReturnForProductStocksFromOldToNew(
        IEnumerable<StockUnitsRemovalDto> removalRequests, string deliveryAddress);
    Result RequestDisposalForProductStocksFromOldToNew(
        IEnumerable<StockUnitsRemovalDto> removalRequests);
    Result RequestReturnForLots(IEnumerable<string> lotNumbers, string deliveryAddress);
    Result RequestDisposalForLots(IEnumerable<string> lotNumbers);
    void RequestDisposalForLotsUnfulfillableBeyondPolicyDuration();
}

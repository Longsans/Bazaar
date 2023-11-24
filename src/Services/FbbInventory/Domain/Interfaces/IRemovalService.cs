namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IRemovalService
{
    Result RequestReturnForProductStocks(
        IEnumerable<StockUnitsRemovalDto> removalRequests, string deliveryAddress);
    Result RequestDisposalForProductStocks(
        IEnumerable<StockUnitsRemovalDto> removalRequests);
    Result RequestReturnForLots(IEnumerable<string> lotNumbers, string deliveryAddress);
    Result RequestDisposalForLots(IEnumerable<string> lotNumbers);
    void RequestDisposalForLotsUnfulfillableBeyondPolicyDuration();
}

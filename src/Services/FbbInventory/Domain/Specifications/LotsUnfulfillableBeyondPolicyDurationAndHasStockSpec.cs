namespace Bazaar.FbbInventory.Domain.Specifications;

public class LotsUnfulfillableBeyondPolicyDurationAndHasStockSpec : Specification<Lot>
{
    public LotsUnfulfillableBeyondPolicyDurationAndHasStockSpec()
    {
        Query.Include(x => x.ProductInventory.Lots)
            .Include(x => x.ProductInventory.SellerInventory)
            .Where(x => x.DateUnitsBecameUnfulfillable != null)
            .PostProcessingAction(lots => lots.Where(
                x => x.IsUnitsUnfulfillableBeyondPolicyDuration && x.HasUnitsInStock));
    }
}

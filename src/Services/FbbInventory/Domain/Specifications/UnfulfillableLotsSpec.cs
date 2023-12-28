namespace Bazaar.FbbInventory.Domain.Specifications;

public class UnfulfillableLotsSpec : Specification<Lot>
{
    public UnfulfillableLotsSpec()
    {
        Query.Include(x => x.ProductInventory.Lots)
            .Include(x => x.ProductInventory.SellerInventory)
            .Where(x => x.IsUnitsUnfulfillable);
    }
}

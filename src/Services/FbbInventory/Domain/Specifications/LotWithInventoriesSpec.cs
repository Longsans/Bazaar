namespace Bazaar.FbbInventory.Domain.Specifications;

public class LotWithInventoriesSpec : SingleResultSpecification<Lot>
{
    public LotWithInventoriesSpec(int id)
    {
        Query.Include(x => x.ProductInventory.Lots)
            .Include(x => x.ProductInventory.SellerInventory)
            .Where(x => x.Id == id);
    }

    public LotWithInventoriesSpec(string lotNumber)
    {
        Query.Include(x => x.ProductInventory.Lots)
            .Include(x => x.ProductInventory.SellerInventory)
            .Where(x => x.LotNumber == lotNumber);
    }
}

namespace Bazaar.FbbInventory.Domain.Specifications;

public class SellerInventoryWithProductsAndLotsSpec : SingleResultSpecification<SellerInventory>
{
    public SellerInventoryWithProductsAndLotsSpec(int id)
    {
        Query.Include(x => x.ProductInventories)
            .ThenInclude(x => x.Lots)
            .Where(x => x.Id == id);
    }

    public SellerInventoryWithProductsAndLotsSpec(string sellerId)
    {
        Query.Include(x => x.ProductInventories)
            .ThenInclude(x => x.Lots)
            .Where(x => x.SellerId == sellerId);
    }
}

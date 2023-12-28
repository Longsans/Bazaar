namespace Bazaar.FbbInventory.Domain.Specifications;

public class ProductInventoryWithLotsAndSellerSpec
    : SingleResultSpecification<ProductInventory>
{
    public ProductInventoryWithLotsAndSellerSpec(int id)
    {
        Query.Include(x => x.Lots)
            .Include(x => x.SellerInventory)
            .Where(x => x.Id == id);
    }

    public ProductInventoryWithLotsAndSellerSpec(string productId)
    {
        Query.Include(x => x.Lots)
            .Include(x => x.SellerInventory)
            .Where(x => x.ProductId == productId);
    }
}

namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemByProductIdSpec : SingleResultSpecification<CatalogItem>
{
    public CatalogItemByProductIdSpec(string productId, bool includeDeleted = false)
    {
        Query.Where(x => x.ProductId == productId);

        if (!includeDeleted)
            Query.Where(x => x.ListingStatus != ListingStatus.Deleted);
    }
}

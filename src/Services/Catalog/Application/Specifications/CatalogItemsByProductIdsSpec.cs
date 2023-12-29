namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsByProductIdsSpec : Specification<CatalogItem>
{
    public CatalogItemsByProductIdsSpec(string[] productIds, bool includeDeleted)
    {
        Query.Where(x => productIds.Contains(x.ProductId));

        if (!includeDeleted)
            Query.Where(x => x.ListingStatus != ListingStatus.Deleted);
    }
}

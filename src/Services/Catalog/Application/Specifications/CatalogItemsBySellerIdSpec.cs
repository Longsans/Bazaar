namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsBySellerIdSpec : Specification<CatalogItem>
{
    public CatalogItemsBySellerIdSpec(string sellerId, bool includeDeleted = false)
    {
        Query.Where(x => x.SellerId == sellerId);

        if (!includeDeleted)
            Query.Where(x => x.ListingStatus != ListingStatus.Deleted);
    }
}

namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsByProductIdSpec : Specification<CatalogItem>
{
    public CatalogItemsByProductIdSpec(IEnumerable<string> productIds, bool includeDeleted = false)
    {
        Query.Include(x => x.MainDepartment)
            .Include(x => x.Subcategory)
            .Where(x => productIds.Contains(x.ProductId));

        if (!includeDeleted)
            Query.Where(x => x.ListingStatus != ListingStatus.Deleted);
    }
}

namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsByNameSpec : Specification<CatalogItem>
{
    public CatalogItemsByNameSpec(string nameSubstring, bool includeDeleted)
    {
        Query.Include(x => x.MainDepartment)
            .Include(x => x.Subcategory)
            .Where(x => x.ProductName.Contains(nameSubstring));

        if (!includeDeleted)
            Query.Where(x => x.ListingStatus != ListingStatus.Deleted);
    }
}

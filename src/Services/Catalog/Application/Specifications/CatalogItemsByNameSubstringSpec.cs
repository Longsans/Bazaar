namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsByNameSubstringSpec : Specification<CatalogItem>
{
    public CatalogItemsByNameSubstringSpec(string nameSubstring, bool includeDeleted)
    {
        Query.Where(x => x.ProductName.Contains(nameSubstring));

        if (!includeDeleted)
            Query.Where(x => x.ListingStatus != ListingStatus.Deleted);
    }
}

namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsBySubcategoryAndNameSpec : Specification<CatalogItem>
{
    public CatalogItemsBySubcategoryAndNameSpec(int subcategoryId, string? nameSubstring, bool includeDeleted)
    {
        Query.IncludeCategories()
            .Where(x => x.SubcategoryId == subcategoryId
                && (nameSubstring == null || x.ProductName.Contains(nameSubstring)))
            .WithDeleted(includeDeleted);
    }
}

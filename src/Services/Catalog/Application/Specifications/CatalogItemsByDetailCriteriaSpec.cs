namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsByDetailCriteriaSpec : Specification<CatalogItem>
{
    public CatalogItemsByDetailCriteriaSpec(int? categoryId, string? sellerId, string? nameSubstring, bool includeDeleted)
    {
        if (categoryId is null && string.IsNullOrWhiteSpace(sellerId) && string.IsNullOrWhiteSpace(nameSubstring))
        {
            throw new ArgumentException("At least one of the following query parameters must be specified: " +
                "productId, sellerId, partOfName, category");
        }

        Query.IncludeCategories().WithDeleted(includeDeleted);
        if (categoryId is not null)
            Query.Where(x => x.MainDepartmentId == categoryId || x.SubcategoryId == categoryId);

        if (sellerId is not null)
            Query.Where(x => x.SellerId == sellerId);

        if (nameSubstring is not null)
            Query.Where(x => x.ProductName.Contains(nameSubstring));
    }
}

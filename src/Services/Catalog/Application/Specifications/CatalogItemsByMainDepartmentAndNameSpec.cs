namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsByMainDepartmentAndNameSpec : Specification<CatalogItem>
{
    public CatalogItemsByMainDepartmentAndNameSpec(int departmentCategoryId, string? nameSubstring, bool includeDeleted = false)
    {
        Query.IncludeCategories()
            .Where(x => x.MainDepartmentId == departmentCategoryId
                && (nameSubstring == null || x.ProductName.Contains(nameSubstring)))
            .WithDeleted(includeDeleted);
    }
}

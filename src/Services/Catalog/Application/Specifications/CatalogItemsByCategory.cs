namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsByCategory : Specification<CatalogItem>
{
    public CatalogItemsByCategory(int categoryId, bool includeDeleted)
    {
        Query.Include(x => x.MainDepartment)
            .Include(x => x.Subcategory)
            .Where(x => x.MainDepartmentId == categoryId || x.SubcategoryId == categoryId);

        if (!includeDeleted)
        {
            Query.Where(x => !x.IsDeleted);
        }
    }
}

namespace Bazaar.Catalog.Application.Specifications;

public class MainDepartmentCategoriesSpec : Specification<ProductCategory>
{
    public MainDepartmentCategoriesSpec()
    {
        Query.Include(x => x.ChildCategories)
            .Include(x => x.ParentCategory)
            .Where(x => x.ParentCategory == null);
    }
}

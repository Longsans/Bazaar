namespace Bazaar.Catalog.Application.Specifications;

public class ProductCategoryByNameSubstringSpec : Specification<ProductCategory>
{
    public ProductCategoryByNameSubstringSpec(string name, bool includeProducts)
    {
        Query.Include(x => x.ParentCategory)
            .Include(x => x.ChildCategories)
            .Where(x => x.Name.Contains(name));

        if (includeProducts)
        {
            Query.Include(x => x.MainDepartmentProducts)
                .Include(x => x.SubcategoryProducts);
        }
    }
}

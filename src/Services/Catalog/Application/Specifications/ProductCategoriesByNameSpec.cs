namespace Bazaar.Catalog.Application.Specifications;

public class ProductCategoriesByNameSpec : Specification<ProductCategory>
{
    public ProductCategoriesByNameSpec(string name, bool includeProducts = false)
    {
        Query.Include(x => x.ParentCategory)
            .Include(x => x.ChildCategories)
            .IncludeProducts(includeProducts)
            .Where(x => x.Name.Contains(name));
    }
}

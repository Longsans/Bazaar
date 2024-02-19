namespace Bazaar.Catalog.Application.Specifications;

public class ProductCategoryByIdSpec : SingleResultSpecification<ProductCategory>
{
    public ProductCategoryByIdSpec(int id, bool includeProducts = false)
    {
        Query.Include(x => x.ParentCategory)
            .Include(x => x.ChildCategories)
            .IncludeProducts(includeProducts)
            .Where(x => x.Id == id);
    }
}

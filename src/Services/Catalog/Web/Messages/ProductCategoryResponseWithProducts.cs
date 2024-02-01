namespace Bazaar.Catalog.Web.Messages;

public record ProductCategoryResponseWithProducts : ProductCategoryResponse
{
    public CatalogItem[] MainDepartmentProducts { get; private set; }
    public CatalogItem[] SubcategoryProducts { get; private set; }

    public ProductCategoryResponseWithProducts(ProductCategory category) : base(category)
    {
        MainDepartmentProducts = category.MainDepartmentProducts.ToArray();
        SubcategoryProducts = category.SubcategoryProducts.ToArray();
    }
}

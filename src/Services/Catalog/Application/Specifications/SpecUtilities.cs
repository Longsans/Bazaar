namespace Bazaar.Catalog.Application.Specifications;

public static class SpecUtilities
{
    public static ISpecificationBuilder<CatalogItem> IncludeCategories(this ISpecificationBuilder<CatalogItem> builder)
    {
        return builder.Include(x => x.MainDepartment)
            .Include(x => x.Subcategory);
    }

    public static ISpecificationBuilder<CatalogItem> WithDeleted(this ISpecificationBuilder<CatalogItem> builder, bool includeDeleted)
    {
        return includeDeleted ? builder : builder.Where(x => x.ListingStatus != ListingStatus.Deleted);
    }

    public static ISpecificationBuilder<ProductCategory> IncludeProducts(this ISpecificationBuilder<ProductCategory> builder, bool includeProducts = true)
    {
        return includeProducts ? builder.Include(x => x.MainDepartmentProducts).Include(x => x.SubcategoryProducts) : builder;
    }
}

﻿namespace Bazaar.Catalog.Application.Specifications;

public class ProductCategoryByIdSpec : SingleResultSpecification<ProductCategory>
{
    public ProductCategoryByIdSpec(int id, bool includeProducts)
    {
        Query.Include(x => x.ParentCategory)
            .Include(x => x.ChildCategories)
            .Where(x => x.Id == id);

        if (includeProducts)
        {
            Query.Include(x => x.MainDepartmentProducts)
                .Include(x => x.SubcategoryProducts);
        }
    }
}

namespace Bazaar.Catalog.Web.Messages;

public record ProductCategoryResponse
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public ProductCategoryResponse[] ChildCategories { get; private set; }
    public string? ParentCategory { get; private set; }
    public bool IsMainDepartment => ParentCategory is null;

    public ProductCategoryResponse(ProductCategory category)
    {
        Id = category.Id;
        Name = category.Name;
        ChildCategories = category.ChildCategories.Select(x => new ProductCategoryResponse(x)).ToArray();
        ParentCategory = category.ParentCategory?.Name;
    }
}

namespace Bazaar.Catalog.Web.Messages;

public record CreateProductCategoryRequest
{
    public string Name { get; private set; }
    public int? ParentCategoryId { get; private set; }
    public CreateChildCategoryRequest[] ChildCategories { get; private set; }

    public CreateProductCategoryRequest(string name, int? parentCategoryId, CreateChildCategoryRequest[]? childCategories = null)
    {
        Name = name;
        ParentCategoryId = parentCategoryId;
        ChildCategories = childCategories ?? Array.Empty<CreateChildCategoryRequest>();
    }
}

public record CreateChildCategoryRequest
{
    public string Name { get; private set; }
    public CreateChildCategoryRequest[] ChildCategories { get; private set; }

    public CreateChildCategoryRequest(string name, CreateChildCategoryRequest[]? childCategories = null)
    {
        Name = name;
        ChildCategories = childCategories ?? Array.Empty<CreateChildCategoryRequest>();
    }

    public ProductCategory ToCategory() => new(Name, null, ChildCategories.Select(x => x.ToCategory()));
}


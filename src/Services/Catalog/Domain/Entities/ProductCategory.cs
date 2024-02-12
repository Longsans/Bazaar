namespace Bazaar.Catalog.Domain.Entities;

public class ProductCategory
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    private List<ProductCategory> _childCategories = new();
    public IReadOnlyCollection<ProductCategory> ChildCategories => _childCategories.AsReadOnly();
    public ProductCategory? ParentCategory { get; private set; }
    public int? ParentCategoryId { get; private set; }
    public bool IsMainDepartment => ParentCategory is null;

    private List<CatalogItem> _mainDepartmentProducts = new();
    private List<CatalogItem> _subcategoryProducts = new();
    public IReadOnlyCollection<CatalogItem> MainDepartmentProducts => _mainDepartmentProducts.AsReadOnly();
    public IReadOnlyCollection<CatalogItem> SubcategoryProducts => _subcategoryProducts.AsReadOnly();

    public ProductCategory MainDepartment
    {
        get
        {
            var mainDepartment = this;
            while (mainDepartment.ParentCategory is not null)
            {
                mainDepartment = mainDepartment.ParentCategory;
            }
            return mainDepartment;
        }
    }

    public ProductCategory(string name, ProductCategory? parentCategory, IEnumerable<ProductCategory>? childCategories = null)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentOutOfRangeException(nameof(name));
        ParentCategory = parentCategory;
        _childCategories = childCategories?.ToList() ?? _childCategories;
    }

    private ProductCategory() { }

    public void AddChildCategories(IEnumerable<ProductCategory> childCategories)
    {
        if (childCategories.Any(x => _childCategories.Contains(x)))
        {
            throw new ArgumentException("One of the categories is already a child of this category.");
        }
        _childCategories.AddRange(childCategories);
    }

    public void ChangeName(string name)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentOutOfRangeException(nameof(name));
    }
}

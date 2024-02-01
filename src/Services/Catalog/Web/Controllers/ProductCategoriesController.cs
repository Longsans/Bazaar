namespace Bazaar.Catalog.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductCategoriesController : ControllerBase
{
    private readonly IRepository<ProductCategory> _categoryRepo;

    public ProductCategoriesController(IRepository<ProductCategory> categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<ProductCategoryResponse>> GetById(int categoryId, bool includeProducts)
    {
        var category = await _categoryRepo.SingleOrDefaultAsync(
            new ProductCategoryByIdSpec(categoryId, includeProducts));
        if (category is null)
        {
            return NotFound();
        }
        return includeProducts ? new ProductCategoryResponseWithProducts(category) : new ProductCategoryResponse(category);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductCategoryResponse>>> GetByCriteria(string? nameSubstring = null)
    {
        var queryAction = async () => !string.IsNullOrWhiteSpace(nameSubstring)
            ? await _categoryRepo.ListAsync(new ProductCategoryByNameSubstringSpec(nameSubstring, false))
            : await _categoryRepo.ListAsync(new MainDepartmentCategoriesSpec());

        var categories = await queryAction();
        return categories.Select(x => new ProductCategoryResponse(x)).ToList();
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewCategories(CreateProductCategoryRequest request)
    {
        var parentCategory = await _categoryRepo.GetByIdAsync(request.ParentCategoryId ?? -1);
        if (parentCategory is null && request.ParentCategoryId is not null)
        {
            return NotFound("Parent category not found.");
        }

        var category = new ProductCategory(
            request.Name, parentCategory, request.ChildCategories.Select(x => x.ToCategory()));
        await _categoryRepo.AddAsync(category);
        return CreatedAtAction(nameof(GetById),
            new { categoryId = category.Id },
            new ProductCategoryResponse(category));
    }

    [HttpPatch("{categoryId}")]
    public async Task<IActionResult> UpdateCategoryName(int categoryId, UpdateCategoryRequest request)
    {
        var category = await _categoryRepo.GetByIdAsync(categoryId);
        if (category is null)
        {
            return NotFound();
        }
        category.ChangeName(request.Name);
        await _categoryRepo.UpdateAsync(category);
        return NoContent();
    }
}

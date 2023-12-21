namespace Bazaar.FbbInventory.Web.Controllers;

[Route("api/product-inventories")]
[ApiController]
public class ProductInventoriesController : ControllerBase
{
    private readonly IProductInventoryRepository _productInventoryRepo;

    public ProductInventoriesController(IProductInventoryRepository productInventoryRepo)
    {
        _productInventoryRepo = productInventoryRepo;
    }

    [HttpGet("{productId}")]
    public ActionResult<ProductInventoryResponse> GetByProductId(string productId)
    {
        var productInventory = _productInventoryRepo.GetByProductId(productId);
        if (productInventory == null)
        {
            return NotFound();
        }
        return new ProductInventoryResponse(productInventory);
    }
}

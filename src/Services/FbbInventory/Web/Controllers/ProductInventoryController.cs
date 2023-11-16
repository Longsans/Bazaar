namespace Bazaar.FbbInventory.Web.Controllers;

[Route("api/product-inventories")]
[ApiController]
public class ProductInventoryController : ControllerBase
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly IUpdateProductStockService _updateProductStockService;
    private readonly IDeleteProductInventoryService _deleteService;
    private readonly IInventoryDisposalService _disposalService;

    public ProductInventoryController(
        IProductInventoryRepository productInventoryRepo,
        IUpdateProductStockService updateStockService,
        IDeleteProductInventoryService deleteService,
        IInventoryDisposalService disposalService)
    {
        _productInventoryRepo = productInventoryRepo;
        _updateProductStockService = updateStockService;
        _deleteService = deleteService;
        _disposalService = disposalService;
    }

    [HttpGet("{productId}")]
    public ActionResult<ProductInventory> GetByProductId(string productId)
    {
        var productInventory = _productInventoryRepo.GetByProductId(productId);
        if (productInventory == null)
        {
            return NotFound();
        }
        return productInventory;
    }

    [HttpPatch("{productId}/reduce-stock")]
    public IActionResult ReduceStock(string productId, ReduceProductStockRequest request)
    {
        return _updateProductStockService
            .ReduceStock(productId, request.StockUnitsToReduce)
            .ToActionResult(this);
    }

    [HttpPatch("{productId}/restock")]
    public IActionResult Restock(string productId, RestockProductRequest request)
    {
        return _updateProductStockService
            .Restock(productId, request.UnitsToRestock)
            .ToActionResult(this);
    }

    [HttpDelete("{productId}")]
    public IActionResult Delete(string productId)
    {
        return _deleteService.DeleteProductInventory(productId)
            .ToActionResult(this);
    }

    [HttpPatch]
    public IActionResult MarkOverdueUnfulfillableInventoriesForDisposal(
        bool overdueUnfulfillable, MarkInventoriesToBeDisposedRequest request)
    {
        if (!request.ToBeDisposed)
        {
            return NoContent();
        }
        else if (!overdueUnfulfillable)
        {
            return BadRequest();
        }

        _disposalService.MarkOverdueUnfulfillableInventoriesForDisposal();
        return NoContent();
    }

    [HttpDelete]
    public IActionResult DisposeMarkedInventories(bool toBeDisposed)
    {
        if (toBeDisposed)
        {
            _disposalService.DisposeMarkedInventories();
        }
        return NoContent();
    }
}

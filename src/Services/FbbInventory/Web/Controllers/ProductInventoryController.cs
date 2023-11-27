namespace Bazaar.FbbInventory.Web.Controllers;

[Route("api/product-inventories")]
[ApiController]
public class ProductInventoryController : ControllerBase
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly IUpdateProductStockService _updateProductStockService;
    private readonly IDeleteProductInventoryService _deleteService;
    private readonly IRemovalService _removalService;

    public ProductInventoryController(
        IProductInventoryRepository productInventoryRepo,
        IUpdateProductStockService updateStockService,
        IDeleteProductInventoryService deleteService,
        IRemovalService disposalService)
    {
        _productInventoryRepo = productInventoryRepo;
        _updateProductStockService = updateStockService;
        _deleteService = deleteService;
        _removalService = disposalService;
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

    [HttpPatch("{productId}/reduce-stock")]
    public IActionResult ReduceStock(string productId, ReduceProductStockRequest request)
    {
        return _updateProductStockService
            .ReduceStock(productId,
                request.FulfillableUnits, request.UnfulfillableUnits)
            .ToActionResult(this);
    }

    [HttpPatch("{productId}/add-fulfillable")]
    public IActionResult AddFulfillableStock(string productId, AddFulfillableStockRequest request)
    {
        return _updateProductStockService
            .AddFulfillableStock(productId, request.Units)
            .ToActionResult(this);
    }

    [HttpPatch("{productId}/add-unfulfillable")]
    public IActionResult AddUnfulfillableStock(string productId, AddUnfulfillableStockRequest request)
    {
        return _updateProductStockService
            .AddUnfulfillableStock(productId,
                request.UnfulfillableCategory, request.Units)
            .ToActionResult(this);
    }

    [HttpPatch("request-removal")]
    public IActionResult RequestRemovalForProductStocks(StockUnitsRemovalRequest request)
    {
        if (request.RemovalMethod == RemovalMethod.Return
            && string.IsNullOrWhiteSpace(request.DeliveryAddress))
        {
            return BadRequest(new { error = "Delivery address must be specified for return." });
        }

        var stockRemovalDtos = request.RemovalQuantities.Select(x => new StockUnitsRemovalDto(
                x.ProductId, x.FulfillableUnits, x.UnfulfillableUnits));

        return (request.RemovalMethod == RemovalMethod.Return
            ? _removalService.RequestReturnForProductStocksFromOldToNew(stockRemovalDtos, request.DeliveryAddress)
            : _removalService.RequestDisposalForProductStocksFromOldToNew(stockRemovalDtos))
            .ToActionResult(this);
    }

    [HttpDelete("{productId}")]
    public IActionResult Delete(string productId)
    {
        return _deleteService.DeleteProductInventory(productId)
            .ToActionResult(this);
    }
}

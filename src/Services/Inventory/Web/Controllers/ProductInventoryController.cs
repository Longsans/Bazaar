using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Inventory.Web.Controllers;

[Route("api/product-inventories")]
[ApiController]
public class ProductInventoryController : ControllerBase
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly IUpdateProductStockService _updateProductStockService;

    public ProductInventoryController(
        IProductInventoryRepository productInventoryRepo, IUpdateProductStockService updateStockService)
    {
        _productInventoryRepo = productInventoryRepo;
        _updateProductStockService = updateStockService;
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
}

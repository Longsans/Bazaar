namespace FbbInventory.Web.Controllers;

[Route("api/seller-inventories")]
[ApiController]
public class SellerInventoriesController : ControllerBase
{
    private readonly RemovalService _removalService;
    private readonly IRepository<SellerInventory> _sellerInventories;

    public SellerInventoriesController(RemovalService removalService, IRepository<SellerInventory> sellerInventories)
    {
        _removalService = removalService;
        _sellerInventories = sellerInventories;
    }

    [HttpGet("{sellerId}")]
    public async Task<ActionResult<SellerInventoryResponse>> GetBySellerId(string sellerId)
    {
        var sellerInventory = await _sellerInventories.SingleOrDefaultAsync(
            new SellerInventoryWithProductsAndLotsSpec(sellerId));
        if (sellerInventory is null)
        {
            return NotFound();
        }
        return new SellerInventoryResponse(sellerInventory);
    }

    [HttpPost("{sellerId}/removed-stocks")]
    public async Task<ActionResult<StockIssue>> SendProductStocksForRemoval(
        string sellerId, StockRemovalRequest request)
    {
        if (!Enum.IsDefined(request.RemovalMethod))
        {
            return BadRequest(new
            {
                error = "Removal method does not exist."
            });
        }
        if (request.RemovalMethod == RemovalMethod.Return && string.IsNullOrWhiteSpace(request.DeliveryAddress))
        {
            return BadRequest(new
            {
                error = "Delivery address must be specified for return."
            });
        }
        if (request.RemovalQuantities.Any(x => x.GoodQuantity + x.UnfulfillableQuantity == 0))
        {
            return BadRequest(new
            {
                error = "One or more removal items have zero total removal quantity."
            });
        }

        var removalQuantities = request.RemovalQuantities.Select(x =>
            new OutboundStockQuantity(x.ProductId, x.GoodQuantity, x.UnfulfillableQuantity));

        var result = request.RemovalMethod == RemovalMethod.Return
            ? await _removalService.SendProductStocksForReturn(sellerId, removalQuantities, request.DeliveryAddress!)
            : await _removalService.SendProductStocksForDisposal(sellerId, removalQuantities);

        return result.ToActionResult(this);
    }
}

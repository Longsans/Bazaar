namespace FbbInventory.Web.Controllers;

[Route("api/seller-inventories")]
[ApiController]
public class SellerInventoriesController : ControllerBase
{
    private readonly RemovalService _removalService;

    public SellerInventoriesController(RemovalService removalService)
    {
        _removalService = removalService;
    }

    [HttpPost("{sellerId}/removed-stocks")]
    public async Task<ActionResult<StockIssue>> SendProductStocksForRemoval(
        string sellerId, StockRemovalRequest request)
    {
        if (request.RemovalMethod == RemovalMethod.Return
            && string.IsNullOrWhiteSpace(request.DeliveryAddress))
        {
            return BadRequest(new { error = "Delivery address must be specified for return." });
        }
        if (request.RemovalQuantities.Any(x => x.GoodQuantity + x.UnfulfillableQuantity == 0))
        {
            return BadRequest(new { error = "One or more removal items have zero total removal quantity." });
        }

        var removalQuantities = request.RemovalQuantities.Select(x =>
            new OutboundStockQuantity(x.ProductId, x.GoodQuantity, x.UnfulfillableQuantity));

        return (request.RemovalMethod == RemovalMethod.Return
            ? (await _removalService.SendProductStocksForReturn(
                sellerId, removalQuantities, request.DeliveryAddress!))
            : (await _removalService.SendProductStocksForDisposal(
                sellerId, removalQuantities)))
            .ToActionResult(this);
    }
}

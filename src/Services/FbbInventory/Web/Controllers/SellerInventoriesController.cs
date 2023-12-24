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
    public ActionResult<StockIssue> SendProductStocksForRemoval(string sellerId, StockRemovalRequest request)
    {
        if (request.RemovalMethod == RemovalMethod.Return
            && string.IsNullOrWhiteSpace(request.DeliveryAddress))
        {
            return BadRequest(new { error = "Delivery address must be specified for return." });
        }

        var removalQuantities = request.RemovalQuantities.Select(x =>
            new OutboundStockQuantity(x.ProductId, x.GoodQuantity, x.UnfulfillableQuantity));

        return (request.RemovalMethod == RemovalMethod.Return
            ? _removalService.SendProductStocksForReturn(sellerId, removalQuantities, request.DeliveryAddress!)
            : _removalService.SendProductStocksForDisposal(sellerId, removalQuantities))
            .ToActionResult(this);
    }
}

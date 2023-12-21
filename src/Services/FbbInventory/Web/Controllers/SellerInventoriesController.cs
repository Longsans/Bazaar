namespace FbbInventory.Web.Controllers;

[Route("api/seller-inventories")]
[ApiController]
public class SellerInventoriesController : ControllerBase
{
    private readonly StockTransactionService _stockTxnService;
    private readonly RemovalService _removalService;

    public SellerInventoriesController(
        StockTransactionService stockTxnService, RemovalService removalService)
    {
        _stockTxnService = stockTxnService;
        _removalService = removalService;
    }

    [HttpPost("{sellerId}/received-stocks")]
    public ActionResult<StockReceipt> ReceiveStock(
        string sellerId, IEnumerable<InboundStockQuantity> receiptQuantities)
    {
        return _stockTxnService.ReceiveStock(sellerId, receiptQuantities).ToActionResult(this);
    }

    [HttpPost("{sellerId}/issued-stocks")]
    public ActionResult<StockIssue> IssueStock(
        string sellerId, IssueStockRequest request)
    {
        return _stockTxnService
            .IssueStockByDateOldToNew(sellerId, request.IssueQuantities, request.IssueReason)
            .ToActionResult(this);
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

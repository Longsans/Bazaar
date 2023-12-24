namespace Bazaar.FbbInventory.Web.Controllers;

[Route("api/product-inventories")]
[ApiController]
public class ProductInventoriesController : ControllerBase
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly StockTransactionService _stockTxnService;

    public ProductInventoriesController(
        IProductInventoryRepository productInventoryRepo,
        StockTransactionService stockTxnService)
    {
        _productInventoryRepo = productInventoryRepo;
        _stockTxnService = stockTxnService;
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

    [HttpPost("/api/received-stocks")]
    public ActionResult<StockReceipt> ReceiveStock(
        IEnumerable<InboundStockQuantity> receiptQuantities)
    {
        return _stockTxnService.ReceiveStock(receiptQuantities).ToActionResult(this);
    }

    [HttpPost("/api/issued-stocks")]
    public ActionResult<StockIssue> IssueStock(IssueStockRequest request)
    {
        return _stockTxnService
            .IssueStocksFifo(request.IssueQuantities, request.IssueReason)
            .ToActionResult(this);
    }
}

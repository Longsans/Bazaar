namespace Bazaar.FbbInventory.Web.Controllers;

[Route("api/product-inventories")]
[ApiController]
public class ProductInventoriesController : ControllerBase
{
    private readonly IRepository<ProductInventory> _productInventoryRepo;
    private readonly StockTransactionService _stockTxnService;

    public ProductInventoriesController(
        IRepository<ProductInventory> productInventoryRepo,
        StockTransactionService stockTxnService)
    {
        _productInventoryRepo = productInventoryRepo;
        _stockTxnService = stockTxnService;
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductInventoryResponse>> GetByProductId(string productId)
    {
        var productInventory = await _productInventoryRepo.SingleOrDefaultAsync(
            new ProductInventoryWithLotsAndSellerSpec(productId));
        if (productInventory == null)
        {
            return NotFound();
        }
        return new ProductInventoryResponse(productInventory);
    }

    [HttpPost("/api/received-stocks")]
    public async Task<ActionResult<StockReceipt>> ReceiveStock(
        IEnumerable<InboundStockQuantity> receiptQuantities)
    {
        return (await _stockTxnService.ReceiveStock(receiptQuantities)).ToActionResult(this);
    }

    [HttpPost("/api/issued-stocks")]
    public async Task<ActionResult<StockIssue>> IssueStock(IssueStockRequest request)
    {
        return (await _stockTxnService
            .IssueStocksFifo(request.IssueQuantities, request.IssueReason))
            .ToActionResult(this);
    }
}

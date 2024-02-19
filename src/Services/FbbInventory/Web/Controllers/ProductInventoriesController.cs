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
    public async Task<ActionResult<ProductInventoryWithLotsResponse>> GetByProductId(string productId)
    {
        var productInventory = await _productInventoryRepo.SingleOrDefaultAsync(
            new ProductInventoryWithLotsAndSellerSpec(productId));
        if (productInventory == null)
        {
            return NotFound();
        }
        return new ProductInventoryWithLotsResponse(productInventory);
    }

    [HttpPost("/api/received-stocks")]
    public async Task<ActionResult<StockReceipt>> ReceiveStock(
        IEnumerable<InboundStockQuantity> receiptQuantities)
    {
        return (await _stockTxnService.ReceiveStock(receiptQuantities)).ToActionResult(this);
    }

    [HttpPost("/api/sales")]
    public async Task<ActionResult<StockIssue>> IssueStocksForSale(
        IEnumerable<SaleQuantity> saleQuantities)
    {
        foreach (var saleQuantity in saleQuantities)
        {
            if (string.IsNullOrWhiteSpace(saleQuantity.ProductId))
            {
                return BadRequest(new { error = "An item in sale request has no product ID." });
            }
            if (saleQuantity.Quantity == 0)
            {
                return BadRequest(new { error = "An item in sale request has no quantity." });
            }
        }
        var issueQuantities = saleQuantities.Select(x =>
            new OutboundStockQuantity(x.ProductId, x.Quantity, 0u));

        return (await _stockTxnService
            .IssueStocksFifo(issueQuantities, StockIssueReason.Sale))
            .ToActionResult(this);
    }
}

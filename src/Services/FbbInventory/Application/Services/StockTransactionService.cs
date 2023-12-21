namespace Bazaar.FbbInventory.Application.Services;

public class StockTransactionService
{
    private readonly IQualityInspectionService _qualityInspectionService;
    private readonly ISellerInventoryRepository _sellerInvenRepo;
    private readonly IProductInventoryRepository _productInvenRepo;
    private readonly IEventBus _eventBus;

    public StockTransactionService(
        IQualityInspectionService qualityInspectionService,
        ISellerInventoryRepository sellerInvenRepo,
        IProductInventoryRepository productInvenRepo,
        IEventBus eventBus)
    {
        _qualityInspectionService = qualityInspectionService;
        _sellerInvenRepo = sellerInvenRepo;
        _productInvenRepo = productInvenRepo;
        _eventBus = eventBus;
    }

    public Result<StockReceipt> ReceiveStock(string sellerId,
        IEnumerable<InboundStockQuantity> receiptQuantities)
    {
        if (receiptQuantities.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(receiptQuantities),
                ErrorMessage = "Quantities cannot contain duplicate products."
            });
        }
        if (receiptQuantities.Any(x => x.Quantity == 0))
        {
            return Result.Invalid(new ValidationError()
            {
                Identifier = nameof(receiptQuantities),
                ErrorMessage = "Each quantity must have at least 1 unit."
            });
        }

        var sellerInven = _sellerInvenRepo.GetWithProductsBySellerId(sellerId);
        if (sellerInven == null)
        {
            return Result.NotFound("Seller inventory not found for seller ID.");
        }
        var inspectionReport = _qualityInspectionService.ConductInspection(receiptQuantities);
        var receiptItems = inspectionReport.Items.Select(x => new StockReceiptItem(
            x.ProductId, x.GoodQuantity, x.DefectiveQuantity, x.WarehouseDamagedQuantity));
        var receipt = new StockReceipt(receiptItems);

        try
        {
            sellerInven.ReceiveStock(receipt);
        }
        catch (Exception ex)
        {
            return Result.Conflict(ex.Message);
        }
        _sellerInvenRepo.Update(sellerInven);

        var receivedStockItems = receiptItems.Select(x => new ReceivedStockEventItem(
            x.ProductId, x.GoodQuantity, x.DefectiveQuantity, x.WarehouseDamagedQuantity));
        var stockReceivedEvent = new StockReceivedIntegrationEvent(receipt.DateOfReceipt, receivedStockItems);
        _eventBus.Publish(stockReceivedEvent);
        return receipt;
    }

    public Result<StockIssue> IssueStockByDateOldToNew(string sellerId,
        IEnumerable<OutboundStockQuantity> issueQuantities, StockIssueReason issueReason)
    {
        if (issueQuantities.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(issueQuantities),
                ErrorMessage = "Quantities cannot contain duplicate products."
            });
        }
        if (issueQuantities.Any(x => x.GoodQuantity + x.UnfulfillableQuantity == 0))
        {
            return Result.Invalid(new ValidationError()
            {
                Identifier = nameof(issueQuantities),
                ErrorMessage = "Each quantity must have at least 1 unit."
            });
        }

        var sellerInven = _sellerInvenRepo.GetWithProductsBySellerId(sellerId);
        if (sellerInven == null)
        {
            return Result.NotFound("Seller inventory not found for seller ID.");
        }

        try
        {
            var issue = sellerInven.IssueStock(issueQuantities, issueReason);
            _sellerInvenRepo.Update(sellerInven);

            var issuedItems = issueQuantities.Select(x =>
            new IssuedStockEventItem(x.ProductId, x.GoodQuantity, x.UnfulfillableQuantity));
            var stockIssuedEvent = new StockIssuedIntegrationEvent(
                DateTime.Now.Date, issueReason, issuedItems);
            _eventBus.Publish(stockIssuedEvent);
            return Result.Success(issue);
        }
        catch (Exception ex)
        {
            return Result.Conflict(ex.Message);
        }
    }
}
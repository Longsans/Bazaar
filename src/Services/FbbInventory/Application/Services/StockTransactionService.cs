namespace Bazaar.FbbInventory.Application.Services;

public class StockTransactionService
{
    private readonly IQualityInspectionService _qualityInspectionService;
    private readonly IRepository<ProductInventory> _productInvenRepo;
    private readonly IEventBus _eventBus;

    public StockTransactionService(
        IQualityInspectionService qualityInspectionService,
        IRepository<ProductInventory> productInvenRepo,
        IEventBus eventBus)
    {
        _qualityInspectionService = qualityInspectionService;
        _productInvenRepo = productInvenRepo;
        _eventBus = eventBus;
    }

    public async Task<Result<StockReceipt>> ReceiveStock(
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

        var inspectionReport = _qualityInspectionService.ConductInspection(receiptQuantities);
        var receiptItems = inspectionReport.Items.Select(x => new StockReceiptItem(
            x.ProductId, x.GoodQuantity, x.DefectiveQuantity, x.WarehouseDamagedQuantity));
        var receipt = new StockReceipt(receiptItems);

        var updatedInventories = new List<ProductInventory>();
        foreach (var receiptItem in receiptItems)
        {
            var inventory = await _productInvenRepo.SingleOrDefaultAsync(
                new ProductInventoryWithLotsAndSellerSpec(receiptItem.ProductId));
            if (inventory == null)
            {
                return Result.NotFound(
                    $"Product inventory not found for product ID {receiptItem.ProductId}");
            }
            try
            {
                if (receiptItem.GoodQuantity > 0)
                {
                    inventory.ReceiveGoodStock(receiptItem.GoodQuantity);
                }
                if (receiptItem.DefectiveQuantity > 0)
                {
                    inventory.AddUnfulfillableStock(receiptItem.DefectiveQuantity,
                        DateTime.Now.Date, DateTime.Now.Date, UnfulfillableCategory.Defective);
                }
                if (receiptItem.WarehouseDamagedQuantity > 0)
                {
                    inventory.AddUnfulfillableStock(receiptItem.WarehouseDamagedQuantity,
                        DateTime.Now.Date, DateTime.Now.Date, UnfulfillableCategory.WarehouseDamaged);
                }
            }
            catch (Exception ex)
            {
                return Result.Conflict(ex.Message);
            }
            updatedInventories.Add(inventory);
        }
        await _productInvenRepo.UpdateRangeAsync(updatedInventories);

        var receivedStockItems = receiptItems.Select(x => new ReceivedStockEventItem(
            x.ProductId, x.GoodQuantity, x.DefectiveQuantity, x.WarehouseDamagedQuantity));
        var stockReceivedEvent = new StockReceivedIntegrationEvent(receipt.DateOfReceipt, receivedStockItems);
        _eventBus.Publish(stockReceivedEvent);
        return receipt;
    }

    public async Task<Result<StockIssue>> IssueStocksFifo(
        IEnumerable<OutboundStockQuantity> issueQuantities, StockIssueReason issueReason)
    {
        if (!issueQuantities.Any())
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(issueQuantities),
                ErrorMessage = "Issue quantities empty."
            });
        }
        if (issueQuantities.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(issueQuantities),
                ErrorMessage = "Quantities cannot contain duplicate products."
            });
        }

        var issue = new StockIssue(issueReason);
        var updatedInventories = new List<ProductInventory>();
        try
        {
            foreach (var issueQuantity in issueQuantities)
            {
                var inventory = await _productInvenRepo.SingleOrDefaultAsync(
                    new ProductInventoryWithLotsAndSellerSpec(issueQuantity.ProductId));
                if (inventory == null)
                {
                    return Result.NotFound(
                        $"Product inventory not found for product ID {issueQuantity.ProductId}");
                }

                if (issueQuantity.GoodQuantity > 0)
                {
                    var demandedLots = inventory.GetGoodLotsFifoForStockDemand(issueQuantity.GoodQuantity);
                    IssueStockFromLots(demandedLots, issueQuantity.GoodQuantity, ref issue);
                }
                if (issueQuantity.UnfulfillableQuantity > 0)
                {
                    var demandedLots = inventory.GetUnfulfillabbleLotsFifoForStockDemand(
                        issueQuantity.UnfulfillableQuantity);
                    IssueStockFromLots(demandedLots, issueQuantity.UnfulfillableQuantity, ref issue);
                }
                inventory.RemoveEmptyLots();
                updatedInventories.Add(inventory);
            }
        }
        catch (Exception ex)
        {
            return Result.Conflict(ex.Message);
        }

        await _productInvenRepo.UpdateRangeAsync(updatedInventories);
        var issuedItems = issueQuantities.Select(x =>
            new IssuedStockEventItem(x.ProductId, x.GoodQuantity, x.UnfulfillableQuantity));
        var stockIssuedEvent = new StockIssuedIntegrationEvent(
            DateTime.Now.Date, issueReason, issuedItems);
        _eventBus.Publish(stockIssuedEvent);
        return Result.Success(issue);
    }

    private static void IssueStockFromLots(
        IEnumerable<Lot> lots, uint quantity, ref StockIssue currentIssue)
    {
        foreach (var lot in lots)
        {
            var issueUnits = Math.Min(quantity, lot.UnitsInStock);
            var lotIssue = lot.IssueUnits(issueUnits, currentIssue.IssueReason);
            currentIssue = currentIssue.AddItems(lotIssue.Items);
            quantity -= issueUnits;
        }
    }
}
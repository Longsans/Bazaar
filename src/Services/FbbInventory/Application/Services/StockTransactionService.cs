namespace Bazaar.FbbInventory.Application.Services;

public class StockTransactionService
{
    private readonly IStockInspectionService _qualityInspectionService;
    private readonly IRepository<ProductInventory> _productInventories;
    private readonly IEventBus _eventBus;

    public StockTransactionService(
        IStockInspectionService qualityInspectionService,
        IRepository<ProductInventory> productInvenRepo,
        IEventBus eventBus)
    {
        _qualityInspectionService = qualityInspectionService;
        _productInventories = productInvenRepo;
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

        var inspectionReport = await _qualityInspectionService.ConductInspection(receiptQuantities);
        var receiptItems = inspectionReport.Items.Select(x => new StockReceiptItem(
            x.ProductId, x.GoodQuantity, x.DefectiveQuantity, x.WarehouseDamagedQuantity));
        var receipt = new StockReceipt(receiptItems);

        var updatedInventories = new List<ProductInventory>();
        foreach (var receiptItem in receiptItems)
        {
            var inventory = await _productInventories.SingleOrDefaultAsync(
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
        await _productInventories.UpdateRangeAsync(updatedInventories);

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

        var issueItems = new List<StockIssueItem>();
        var updatedInventories = new List<ProductInventory>();
        try
        {
            foreach (var issueQuantity in issueQuantities)
            {
                var inventory = await _productInventories.SingleOrDefaultAsync(
                    new ProductInventoryWithLotsAndSellerSpec(issueQuantity.ProductId));
                if (inventory == null)
                {
                    return Result.NotFound(
                        $"Product inventory not found for product ID {issueQuantity.ProductId}");
                }

                if (issueQuantity.GoodQuantity > 0)
                {
                    var demandedLots = inventory.GetGoodLotsFifoForStockDemand(issueQuantity.GoodQuantity);
                    var lotsIssue = IssueStockFromLots(
                        demandedLots, issueQuantity.GoodQuantity, issueReason);
                    issueItems.AddRange(lotsIssue.Items);
                }
                if (issueQuantity.UnfulfillableQuantity > 0)
                {
                    var demandedLots = inventory.GetUnfulfillabbleLotsFifoForStockDemand(
                        issueQuantity.UnfulfillableQuantity);
                    var lotsIssue = IssueStockFromLots(
                        demandedLots, issueQuantity.UnfulfillableQuantity, issueReason);
                    issueItems.AddRange(lotsIssue.Items);
                }
                inventory.RemoveEmptyLots();
                updatedInventories.Add(inventory);
            }
        }
        catch (Exception ex)
        {
            return Result.Conflict(ex.Message);
        }

        await _productInventories.UpdateRangeAsync(updatedInventories);
        var issue = new StockIssue(issueItems, issueReason);
        var issueEventItems = issueQuantities.Select(x =>
            new IssuedStockEventItem(x.ProductId, x.GoodQuantity, x.UnfulfillableQuantity));
        var stockIssuedEvent = new StockIssuedIntegrationEvent(
            DateTime.Now.Date, issueReason, issueEventItems);
        _eventBus.Publish(stockIssuedEvent);
        return Result.Success(issue);
    }

    public async Task<Result<StockCheckReport>> CheckStockForIssuance(IEnumerable<OutboundStockQuantity> quantities)
    {
        if (!quantities.Any())
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(quantities),
                ErrorMessage = "Quantities empty."
            });
        }
        if (quantities.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(quantities),
                ErrorMessage = "Quantities cannot contain duplicate products."
            });
        }

        var reportItems = await Task.WhenAll(quantities.Select(async quantity =>
        {
            var inventory = await _productInventories.SingleOrDefaultAsync(
                new ProductInventoryWithLotsAndSellerSpec(quantity.ProductId));
            return inventory is null
                ? StockCheckReportItem.UnlistedItem(quantity.ProductId)
                : new StockCheckReportItem(inventory.ProductId, quantity.GoodQuantity, quantity.UnfulfillableQuantity,
                    inventory.FulfillableUnits + inventory.StrandedUnits, inventory.UnfulfillableUnits);
        }));
        return new StockCheckReport(reportItems);
    }

    private static StockIssue IssueStockFromLots(
        IEnumerable<Lot> lots, uint quantity, StockIssueReason issueReason)
    {
        StockIssue? issue = null;
        foreach (var lot in lots)
        {
            var issueUnits = Math.Min(quantity, lot.UnitsInStock);
            var lotIssue = lot.IssueUnits(issueUnits, issueReason);
            issue = issue?.AddItems(lotIssue.Items) ?? lotIssue;
            quantity -= issueUnits;
        }
        return issue!;
    }
}
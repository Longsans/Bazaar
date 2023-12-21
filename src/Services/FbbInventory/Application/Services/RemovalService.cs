namespace Bazaar.FbbInventory.Application.Services;

public class RemovalService
{
    private readonly StockTransactionService _stockTransactionService;
    private readonly ILotRepository _lotRepo;
    private readonly IEventBus _eventBus;

    public RemovalService(ILotRepository lotRepo, IEventBus eventBus,
        StockTransactionService updateStockService)
    {
        _lotRepo = lotRepo;
        _eventBus = eventBus;
        _stockTransactionService = updateStockService;
    }

    public Result<StockIssue> SendProductStocksForReturn(string sellerId,
        IEnumerable<OutboundStockQuantity> returnQuantities, string deliveryAddress)
    {
        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(deliveryAddress),
                ErrorMessage = "Delivery address cannot be empty."
            });
        }

        var result = _stockTransactionService.IssueStockByDateOldToNew(
            sellerId, returnQuantities, StockIssueReason.Return);
        if (!result.IsSuccess)
        {
            return result;
        }

        var returnLotQuantities = result.Value.Items.Select(x => new LotQuantity(x.LotNumber, x.Quantity));
        _eventBus.Publish(new LotQuantitiesSentForReturnIntegrationEvent(
            returnLotQuantities, deliveryAddress, sellerId));
        return result;
    }

    public Result<StockIssue> SendProductStocksForDisposal(
        string sellerId, IEnumerable<OutboundStockQuantity> disposalQuantities)
    {
        var result = _stockTransactionService.IssueStockByDateOldToNew(
            sellerId, disposalQuantities, StockIssueReason.Disposal);
        if (!result.IsSuccess)
        {
            return result;
        }

        var lotDisposalQuantities = result.Value.Items.Select(x =>
            new DisposalLotQuantity(x.LotNumber, x.Quantity, sellerId));
        _eventBus.Publish(new LotQuantitiesSentForDisposalIntegrationEvent(lotDisposalQuantities, false));
        return result;
    }

    public void SendLotsUnfulfillableBeyondPolicyDurationForDisposal()
    {
        // this needs to be refactored into specs...
        // and also reduce joins to improve performance.
        var lotsToSendForRemoval = _lotRepo.GetUnfulfillables()
            .Where(x => x.IsUnfulfillableBeyondPolicyDuration && x.HasUnitsInStock)
            .ToList();

        var disposalLotUnits = lotsToSendForRemoval.Select(x =>
            new DisposalLotQuantity(x.LotNumber, x.UnitsInStock,
                x.ProductInventory.SellerInventory.SellerId))
            .ToList();

        foreach (var lot in lotsToSendForRemoval)
        {
            lot.SendUnitsForRemoval(lot.UnitsInStock);
        }
        _lotRepo.UpdateRange(lotsToSendForRemoval);
        _eventBus.Publish(new LotQuantitiesSentForDisposalIntegrationEvent(disposalLotUnits, true));
    }

    /// <summary>
    /// Restore units from removal to their corresponding lots and publish <see cref="StockAdjustedIntegrationEvent"/>.
    /// </summary>
    /// <param name="restoreQuantities">The quantities to restore, 
    /// where the key is lot number and the value is quantity to restore.</param>
    public Result RestoreLotUnitsFromRemoval(Dictionary<string, uint> restoreQuantities)
    {
        var lotsWithRestoredUnits = new List<Lot>();
        var categorizedRestoreQuantities = new Dictionary<string,
            (uint GoodQty, uint UnfulfillableQty, bool IsStranded)>();
        foreach (var (lotNumber, restoreQuantity) in restoreQuantities.Select(x => (x.Key, x.Value)))
        {
            var lot = _lotRepo.GetByLotNumber(lotNumber);
            if (lot == null)
            {
                return Result.NotFound($"Lot not found for lot number: {lotNumber}");
            }

            try
            {
                lot.RestoreUnitsFromRemoval(restoreQuantity);
            }
            catch (Exception ex)
            {
                return Result.Conflict(ex.Message);
            }
            lotsWithRestoredUnits.Add(lot);

            var productId = lot.ProductInventory.ProductId;
            uint goodQty = 0;
            uint unfulfillableQty = 0;
            if (lot.IsUnitsUnfulfillable)
            {
                unfulfillableQty = restoreQuantity;
            }
            else
            {
                goodQty = restoreQuantity;
            }

            if (categorizedRestoreQuantities.TryGetValue(productId, out var r))
            {
                goodQty += r.GoodQty;
                unfulfillableQty += r.UnfulfillableQty;
                categorizedRestoreQuantities.Remove(productId);
            }
            categorizedRestoreQuantities.Add(productId,
                (goodQty, unfulfillableQty, lot.ProductInventory.IsStranded));
        }
        _lotRepo.UpdateRange(lotsWithRestoredUnits);
        var restoredEventItems = categorizedRestoreQuantities.Select(x =>
            new AdjustedQuantity(x.Key, x.Value.GoodQty, x.Value.UnfulfillableQty, x.Value.IsStranded));
        _eventBus.Publish(new StockAdjustedIntegrationEvent(
            DateTime.Now.Date, restoredEventItems));
        return Result.Success();
    }
}

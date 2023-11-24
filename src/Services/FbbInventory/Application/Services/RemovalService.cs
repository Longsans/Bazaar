namespace Bazaar.FbbInventory.Application.Services;

public class RemovalService : IRemovalService
{
    private readonly ISellerInventoryRepository _sellerInventoryRepo;
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly ILotRepository _lotRepo;
    private readonly IEventBus _eventBus;

    public RemovalService(
        ISellerInventoryRepository sellerInventoryRepo,
        IProductInventoryRepository productInventoryRepo,
        ILotRepository lotRepo, IEventBus eventBus)
    {
        _sellerInventoryRepo = sellerInventoryRepo;
        _productInventoryRepo = productInventoryRepo;
        _lotRepo = lotRepo;
        _eventBus = eventBus;
    }

    public Result RequestRemovalForStockUnits(
        IEnumerable<StockUnitsRemovalDto> removalRequests, RemovalMethod removalMethod)
    {
        var lotReturnQuantities = new Dictionary<Lot, uint>();
        foreach (var removal in removalRequests)
        {
            var productInventory = _productInventoryRepo.GetByProductId(removal.ProductId);
            if (productInventory == null)
            {
                return Result.NotFound($"Product inventory not found. ID: {removal.ProductId}.");
            }

            LabelLotUnitsForRemovalAndRecordResult(
                productInventory.FulfillableLots
                    .Where(x => x.HasUnitsInStock)
                    .OrderBy(x => x.DateEnteredStorage),
                removal.FulfillableUnits, lotReturnQuantities);

            LabelLotUnitsForRemovalAndRecordResult(
                productInventory.UnfulfillableLots
                    .Where(x => x.HasUnitsInStock)
                    .OrderBy(x => x.DateUnfulfillableSince),
                removal.UnfulfillableUnits, lotReturnQuantities);
        }

        _lotRepo.UpdateRange(lotReturnQuantities.Keys);
        if (removalMethod == RemovalMethod.Return)
        {
            var returnItems = lotReturnQuantities.Select(x => new LotUnitsLabeledForReturn(
                x.Key.LotNumber, x.Value, x.Key.ProductInventory.SellerInventory.SellerId));
            foreach (var returnGroup in returnItems.GroupBy(x => x.InventoryOwnerId))
            {
                _eventBus.Publish(new LotUnitsLabeledForReturnIntegrationEvent(returnGroup));
            }
        }
        else
        {
            var returnItems = lotReturnQuantities.Select(x => new LotUnitsLabeledForDisposal(
                x.Key.LotNumber, x.Value, x.Key.ProductInventory.SellerInventory.SellerId));
            foreach (var returnGroup in returnItems.GroupBy(x => x.InventoryOwnerId))
            {
                _eventBus.Publish(new LotUnitsLabeledForDisposalIntegrationEvent(
                    returnGroup, false));
            }
        }
        return Result.Success();
    }

    public Result RequestRemovalForLots(IEnumerable<string> lotNumbers, RemovalMethod removalMethod)
    {
        var hasDuplicateLotNumbers = lotNumbers.GroupBy(x => x)
            .Any(g => g.Count() > 1);
        if (hasDuplicateLotNumbers)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(lotNumbers),
                ErrorMessage = "Lot numbers cannot contain duplicates."
            });
        }

        var lots = _lotRepo.GetManyByLotNumber(lotNumbers);
        var notFoundLotNumber = lotNumbers.FirstOrDefault(x =>
            !lots.Select(l => l.LotNumber).Contains(x));
        if (notFoundLotNumber != null)
        {
            return Result.NotFound(
                "Lot not found for lot number: {notFoundLotNumber}");
        }

        var lotsBeforeRemoval = lots.Select(x =>
            new
            {
                x.LotNumber,
                x.UnitsInStock,
                x.ProductInventory.SellerInventory.SellerId
            }).ToList();

        try
        {
            foreach (var lot in lots)
            {
                lot.LabelUnitsInStockForRemoval(lot.UnitsInStock);
            }
            _lotRepo.UpdateRange(lots);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = ex.ParamName,
                ErrorMessage = ex.Message
            });
        }
        catch (NotEnoughStockException ex)
        {
            return Result.Conflict(ex.Message);
        }

        foreach (var lotGroup in lotsBeforeRemoval.GroupBy(x => x.SellerId))
        {
            IntegrationEvent @event = removalMethod == RemovalMethod.Disposal
                ? new LotUnitsLabeledForDisposalIntegrationEvent(
                    lotGroup.Select(x => new LotUnitsLabeledForDisposal(
                        x.LotNumber, x.UnitsInStock, x.SellerId)), false)
                : new LotUnitsLabeledForReturnIntegrationEvent(
                    lotGroup.Select(x => new LotUnitsLabeledForReturn(
                        x.LotNumber, x.UnitsInStock, x.SellerId)));

            _eventBus.Publish(@event);
        }
        return Result.Success();
    }

    public void RequestDisposalForLotsUnfulfillableBeyondPolicyDuration()
    {
        // this needs to be refactored into specs
        var lotsToBeRemoved = _lotRepo.GetUnfulfillables()
            .Where(x => x.IsUnfulfillableBeyondPolicyDuration && x.HasUnitsInStock)
            .ToList();

        // this is bad
        var sellersWithLotsToBeRemoved = _sellerInventoryRepo.GetAll()
            .Join(lotsToBeRemoved, s => s.SellerId,
                lot => lot.ProductInventory.SellerInventory.SellerId,
                (s, lot) => new { s.SellerId, Lot = lot });

        var disposeQuantities = sellersWithLotsToBeRemoved.Select(x =>
            new LotUnitsLabeledForDisposal(x.Lot.LotNumber, x.Lot.UnitsInStock, x.SellerId))
            .ToList();

        foreach (var lot in sellersWithLotsToBeRemoved.Select(x => x.Lot))
        {
            lot.LabelUnitsInStockForRemoval(lot.UnitsInStock);
        }
        _lotRepo.UpdateRange(lotsToBeRemoved);

        foreach (var disposeQtsGroup in disposeQuantities.GroupBy(x => x.InventoryOwnerId))
        {
            _eventBus.Publish(new LotUnitsLabeledForDisposalIntegrationEvent(
                disposeQtsGroup, true));
        }
    }

    #region Helpers
    private static void LabelLotUnitsForRemovalAndRecordResult(
        IEnumerable<Lot> lots, uint unitsToLabel, Dictionary<Lot, uint> removalRecords)
    {
        foreach (var unfulfillableLot in lots)
        {
            var unitsToRemove = unitsToLabel > unfulfillableLot.UnitsInStock
                ? unfulfillableLot.UnitsInStock
                : unitsToLabel;
            unfulfillableLot.LabelUnitsInStockForRemoval(unitsToRemove);
            unitsToLabel -= unitsToRemove;
            removalRecords.Add(unfulfillableLot, unitsToRemove);

            if (unitsToLabel == 0)
                return;
        }
    }
    #endregion
}

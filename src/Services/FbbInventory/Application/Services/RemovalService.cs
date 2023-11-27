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

    public Result RequestReturnForProductStocksFromOldToNew(
        IEnumerable<StockUnitsRemovalDto> removalRequests, string deliveryAddress)
    {
        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(deliveryAddress),
                ErrorMessage = "Delivery address cannot be empty."
            });
        }

        return RequestRemovalForProductStocksFromOldToNew(removalRequests, deliveryAddress);
    }

    public Result RequestDisposalForProductStocksFromOldToNew(
        IEnumerable<StockUnitsRemovalDto> removalRequests)
    {
        return RequestRemovalForProductStocksFromOldToNew(removalRequests, null);
    }

    public Result RequestReturnForLots(IEnumerable<string> lotNumbers,
        string deliveryAddress)
    {
        if (string.IsNullOrWhiteSpace(deliveryAddress))
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(deliveryAddress),
                ErrorMessage = "Delivery address cannot be empty."
            });
        }

        return RequestRemovalForLots(lotNumbers, deliveryAddress);
    }

    public Result RequestDisposalForLots(IEnumerable<string> lotNumbers)
    {
        return RequestRemovalForLots(lotNumbers, null);
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
    private Result RequestRemovalForProductStocksFromOldToNew(
        IEnumerable<StockUnitsRemovalDto> removalRequests, string? deliveryAddress)
    {
        var hasDuplicateProducts = removalRequests.GroupBy(x => x.ProductId)
            .Any(g => g.Count() > 1);
        if (hasDuplicateProducts)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(removalRequests),
                ErrorMessage = "Removal requests cannot contain duplicate products"
            });
        }

        var lotsWithLabeledUnits = new List<(Lot Lot, uint LabeledUnits)>();
        foreach (var removal in removalRequests)
        {
            var productInventory = _productInventoryRepo.GetByProductId(removal.ProductId);
            if (productInventory == null)
            {
                return Result.NotFound($"Product inventory not found. ID: {removal.ProductId}.");
            }
            if (productInventory.FulfillableUnitsInStock < removal.FulfillableUnits)
            {
                return Result.Conflict(
                    $"Not enough fulfillable stock for product: {productInventory.ProductId}");
            }
            if (productInventory.UnfulfillableUnitsInStock < removal.UnfulfillableUnits)
            {
                return Result.Conflict(
                    $"Not enough unfulfillable stock for product: {productInventory.ProductId}");
            }

            var fulfillableLotsWithLabeledUnits = LabelLotUnitsForRemoval(
                productInventory.FulfillableLots
                    .Where(x => x.HasUnitsInStock)
                    .OrderBy(x => x.DateEnteredStorage),
                removal.FulfillableUnits);

            var unfulfillableLotsWithLabeledUnits = LabelLotUnitsForRemoval(
                productInventory.UnfulfillableLots
                    .Where(x => x.HasUnitsInStock)
                    .OrderBy(x => x.DateUnfulfillableSince),
                removal.UnfulfillableUnits);
            lotsWithLabeledUnits.AddRange(fulfillableLotsWithLabeledUnits);
            lotsWithLabeledUnits.AddRange(unfulfillableLotsWithLabeledUnits);
        }

        _lotRepo.UpdateRange(lotsWithLabeledUnits.Select(x => x.Lot));
        if (deliveryAddress != null)
        {
            foreach (var lotsBySeller in lotsWithLabeledUnits
                .GroupBy(x => x.Lot.ProductInventory.SellerInventory.SellerId))
            {
                _eventBus.Publish(new LotUnitsLabeledForReturnIntegrationEvent(
                    lotsBySeller.Select(x => new UnitsFromLot(x.Lot.LotNumber, x.LabeledUnits)),
                    deliveryAddress, lotsBySeller.Key));
            }
        }
        else
        {
            foreach (var lotsBySeller in lotsWithLabeledUnits
                .GroupBy(x => x.Lot.ProductInventory.SellerInventory.SellerId))
            {
                _eventBus.Publish(new LotUnitsLabeledForDisposalIntegrationEvent(
                    lotsBySeller.Select(x => new LotUnitsLabeledForDisposal(
                        x.Lot.LotNumber, x.LabeledUnits, lotsBySeller.Key)), false));
            }
        }
        return Result.Success();
    }

    private Result RequestRemovalForLots(IEnumerable<string> lotNumbers, string? deliveryAddress)
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
                $"Lot not found for lot number: {notFoundLotNumber}");
        }
        var outOfStockLot = lots.FirstOrDefault(x => !x.HasUnitsInStock);
        if (outOfStockLot != null)
        {
            return Result.Conflict(
                $"Lot has no units in stock to remove: {outOfStockLot.LotNumber}");
        }

        var lotsWithLabeledUnits = lots.Select(x =>
            new
            {
                x.LotNumber,
                x.UnitsInStock,
                x.ProductInventory.SellerInventory.SellerId
            }).ToList();
        foreach (var lot in lots)
        {
            lot.LabelUnitsInStockForRemoval(lot.UnitsInStock);
        }
        _lotRepo.UpdateRange(lots);

        if (deliveryAddress != null)
        {
            foreach (var lotGroup in lotsWithLabeledUnits.GroupBy(x => x.SellerId))
            {
                var @event = new LotUnitsLabeledForReturnIntegrationEvent(
                        lotGroup.Select(x => new UnitsFromLot(
                            x.LotNumber, x.UnitsInStock)), deliveryAddress, lotGroup.Key);

                _eventBus.Publish(@event);
            }
        }
        else
        {
            foreach (var lotGroup in lotsWithLabeledUnits.GroupBy(x => x.SellerId))
            {
                var @event = new LotUnitsLabeledForDisposalIntegrationEvent(
                        lotGroup.Select(x => new LotUnitsLabeledForDisposal(
                            x.LotNumber, x.UnitsInStock, x.SellerId)), false);

                _eventBus.Publish(@event);
            }
        }
        return Result.Success();
    }

    private static List<(Lot lot, uint labeledUnits)> LabelLotUnitsForRemoval(
        IEnumerable<Lot> lots, uint unitsToLabel)
    {
        var lotsWithUnitsLabeled = new List<(Lot, uint)>();
        foreach (var lot in lots)
        {
            var unitsToRemove = unitsToLabel > lot.UnitsInStock
                ? lot.UnitsInStock
                : unitsToLabel;
            lot.LabelUnitsInStockForRemoval(unitsToRemove);
            unitsToLabel -= unitsToRemove;
            lotsWithUnitsLabeled.Add((lot, unitsToRemove));

            if (unitsToLabel == 0)
                break;
        }
        return lotsWithUnitsLabeled;
    }
    #endregion
}

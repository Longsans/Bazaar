namespace Bazaar.FbbInventory.Application.Services;

public class StockAdjustmentService
{
    private readonly Repository<Lot> _lotRepo;
    private readonly Repository<ProductInventory> _productInvenRepo;
    private readonly IEventBus _eventBus;

    public StockAdjustmentService(Repository<Lot> lotRepo,
        Repository<ProductInventory> productInvenRepo, IEventBus eventBus)
    {
        _lotRepo = lotRepo;
        _productInvenRepo = productInvenRepo;
        _eventBus = eventBus;
    }

    public async Task<Result> AdjustStockInLots(IEnumerable<LotAdjustmentQuantityDto> adjustmentQuantities)
    {
        if (!adjustmentQuantities.Any())
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(adjustmentQuantities),
                ErrorMessage = "Adjustment quantities is empty."
            });
        }

        var adjustedLots = new List<Lot>();
        var adjustedInventories = new List<ProductInventory>();
        var adjustedQuantities = new Dictionary<string, AdjustedQuantity>();
        foreach (var adjustmentQty in adjustmentQuantities)
        {
            var lot = await _lotRepo.SingleOrDefaultAsync(
                new LotWithInventoriesSpec(adjustmentQty.LotNumber));
            if (lot == null)
            {
                return Result.NotFound($"Lot not found for lot number {adjustmentQty.LotNumber}");
            }

            try
            {
                lot.AdjustUnits(adjustmentQty.Quantity);
            }
            catch (Exception ex)
            {
                return Result.Conflict(ex.Message);
            }
            adjustedLots.Add(lot);
            adjustedInventories.Add(lot.ProductInventory);

            var productId = lot.ProductInventory.ProductId;
            var goodQuantityAdjusted = lot.IsUnitsUnfulfillable ? 0 : adjustmentQty.Quantity;
            var unfQuantityAdjusted = lot.IsUnitsUnfulfillable ? adjustmentQty.Quantity : 0;
            if (adjustedQuantities.TryGetValue(productId, out var adjustedQuantity))
            {
                goodQuantityAdjusted += adjustedQuantity.GoodQuantity;
                unfQuantityAdjusted += adjustedQuantity.UnfulfillableQuantity;
                adjustedQuantities.Remove(productId);
            }
            adjustedQuantities.Add(productId, new(productId,
                goodQuantityAdjusted, unfQuantityAdjusted, lot.ProductInventory.IsStranded));
        }

        foreach (var inventory in adjustedInventories)
        {
            inventory.RemoveEmptyLots();
        }
        await _lotRepo.UpdateRangeAsync(adjustedLots);
        _eventBus.Publish(new StockAdjustedIntegrationEvent(
            DateTime.Now.Date, adjustedQuantities.Select(x => x.Value)));
        return Result.Success();
    }

    public async Task<Result> MoveLotUnitsIntoUnfulfillableStock(
        string lotNumber, uint quantity, UnfulfillableCategory category)
    {
        if (quantity == 0)
        {
            return Result.Invalid(new ValidationError
            {
                Identifier = nameof(quantity),
                ErrorMessage = "Quantity to adjust cannot be 0."
            });
        }

        var lot = await _lotRepo.SingleOrDefaultAsync(
            new LotWithInventoriesSpec(lotNumber));
        if (lot == null)
        {
            return Result.NotFound("Lot not found for lot number.");
        }
        if (lot.IsUnitsUnfulfillable)
        {
            return Result.Conflict("Lot units are already unfulfillable.");
        }

        var inventory = lot.ProductInventory;
        int signedQuantity = (int)quantity;
        try
        {
            lot.AdjustUnits(-signedQuantity);
            inventory.AddUnfulfillableStock(
                quantity, lot.DateUnitsEnteredStorage, DateTime.Now.Date, category);
        }
        catch (Exception ex)
        {
            return Result.Conflict(ex.Message);
        }
        inventory.RemoveEmptyLots();
        await _productInvenRepo.UpdateAsync(inventory);
        PublishStockAdjustedEvent(inventory, -signedQuantity, signedQuantity);
        return Result.Success();
    }

    public async Task<Result> RenderProductStockStranded(string productId)
    {
        var inventory = await _productInvenRepo.SingleOrDefaultAsync(
            new ProductInventoryWithLotsAndSellerSpec(productId));
        if (inventory == null)
        {
            return Result.NotFound("Product inventory not found for product ID.");
        }

        try
        {
            inventory.TurnStranded();
        }
        catch (Exception ex)
        {
            return Result.Conflict(ex.Message);
        }
        await _productInvenRepo.UpdateAsync(inventory);
        PublishStockAdjustedEvent(inventory, 0, 0);
        return Result.Success();
    }

    public async Task<Result> ConfirmStockStrandingResolved(string productId)
    {
        var inventory = await _productInvenRepo.SingleOrDefaultAsync(
            new ProductInventoryWithLotsAndSellerSpec(productId));
        if (inventory == null)
        {
            return Result.NotFound("Product inventory not found for product ID.");
        }

        try
        {
            inventory.ConfirmStrandingResolved();
        }
        catch (Exception ex)
        {
            return Result.Conflict(ex.Message);
        }
        await _productInvenRepo.UpdateAsync(inventory);
        PublishStockAdjustedEvent(inventory, 0, 0);
        return Result.Success();
    }

    private void PublishStockAdjustedEvent(ProductInventory inventory,
        int goodQuantityAdjusted, int unfulfillableQuantityAdjusted)
    {
        var adjustedItems = new AdjustedQuantity[]
        {
            new(inventory.ProductId, goodQuantityAdjusted, unfulfillableQuantityAdjusted, inventory.IsStranded)
        };
        _eventBus.Publish(new StockAdjustedIntegrationEvent(
            DateTime.Now.Date, adjustedItems));
    }
}

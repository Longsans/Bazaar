namespace Bazaar.FbbInventory.Application.Services;

public class StockAdjustmentService
{
    private readonly ILotRepository _lotRepo;
    private readonly IProductInventoryRepository _productInvenRepo;
    private readonly IEventBus _eventBus;

    public StockAdjustmentService(ILotRepository lotRepo,
        IProductInventoryRepository productInvenRepo, IEventBus eventBus)
    {
        _lotRepo = lotRepo;
        _productInvenRepo = productInvenRepo;
        _eventBus = eventBus;
    }

    public Result AdjustStockInLots(IEnumerable<LotAdjustmentQuantityDto> adjustmentQuantities)
    {
        var adjustedLots = new List<Lot>();
        var adjustedQuantities = new Dictionary<string, AdjustedQuantity>();
        foreach (var adjustmentQty in adjustmentQuantities)
        {
            var lot = _lotRepo.GetByLotNumber(adjustmentQty.LotNumber);
            if (lot == null)
            {
                return Result.NotFound($"Lot not found for lot number {adjustmentQty.LotNumber}");
            }

            var unsignedQuantity = (uint)Math.Abs(adjustmentQty.Quantity);
            try
            {
                if (adjustmentQty.Quantity > 0)
                {
                    lot.IncreaseUnits(unsignedQuantity);
                }
                else
                {
                    lot.ReduceUnits(unsignedQuantity);
                }
            }
            catch (Exception ex)
            {
                return Result.Conflict(ex.Message);
            }
            adjustedLots.Add(lot);

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
        _lotRepo.UpdateRange(adjustedLots);
        _eventBus.Publish(new StockAdjustedIntegrationEvent(
            DateTime.Now.Date, adjustedQuantities.Select(x => x.Value)));
        return Result.Success();
    }

    public Result MoveLotUnitsIntoUnfulfillableStock(
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

        var lot = _lotRepo.GetByLotNumber(lotNumber);
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
            if (inventory.IsStranded)
            {
                inventory.AdjustStrandedStock(lot.DateUnitsEnteredStorage,
                    lot.DateUnitsBecameStranded!.Value, -signedQuantity);
            }
            else
            {
                inventory.AdjustFulfillableStock(lot.DateUnitsEnteredStorage, -signedQuantity);
            }
            inventory.AdjustUnfulfillableStock(lot.DateUnitsEnteredStorage,
                DateTime.Now.Date, category, signedQuantity);
        }
        catch (Exception ex)
        {
            return Result.Conflict(ex.Message);
        }
        inventory.RemoveEmptyLots();
        _productInvenRepo.Update(inventory);
        PublishStockAdjustedEvent(inventory, -signedQuantity, signedQuantity);
        return Result.Success();
    }

    public Result RenderProductStockStranded(string productId)
    {
        var inventory = _productInvenRepo.GetByProductId(productId);
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
        _productInvenRepo.Update(inventory);
        PublishStockAdjustedEvent(inventory, 0, 0);
        return Result.Success();
    }

    public Result ConfirmStockStrandingResolved(string productId)
    {
        var inventory = _productInvenRepo.GetByProductId(productId);
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
        _productInvenRepo.Update(inventory);
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

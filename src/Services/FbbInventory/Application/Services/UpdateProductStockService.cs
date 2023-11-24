namespace Bazaar.FbbInventory.Application.Services;

public class UpdateProductStockService : IUpdateProductStockService
{
    private readonly IProductInventoryRepository _productInventoryRepo;
    private readonly IEventBus _eventBus;

    public UpdateProductStockService(
        IProductInventoryRepository productInventoryRepo, IEventBus eventBus)
    {
        _productInventoryRepo = productInventoryRepo;
        _eventBus = eventBus;
    }

    public Result ReduceStock(
        string productId, uint fulfillableUnits, uint unfulfillableUnits)
    {
        return UpdateStockAndPublishEvent(productId,
            fulfillableUnits + unfulfillableUnits, inventory =>
        {
            if (fulfillableUnits > 0)
            {
                inventory.ReduceFulfillableStock(fulfillableUnits);
            }
            if (unfulfillableUnits > 0)
            {
                inventory.ReduceUnfulfillableStock(unfulfillableUnits);
            }
        });
    }

    public Result AddFulfillableStock(string productId, uint units)
    {
        return UpdateStockAndPublishEvent(productId, units,
            inventory => inventory.AddFulfillableStock(units));
    }

    public Result AddUnfulfillableStock(string productId,
        UnfulfillableCategory category, uint units)
    {
        return UpdateStockAndPublishEvent(productId, units,
            inventory => inventory.AddUnfulfillableStock(category, units));
    }

    public Result LabelStockUnitsForRemoval(string productId,
        uint fulfillableUnits, uint unfulfillableUnits)
    {
        return UpdateStockAndPublishEvent(productId,
            fulfillableUnits + unfulfillableUnits, inventory =>
        {
            if (fulfillableUnits > 0)
            {
                inventory.LabelFulfillableUnitsForRemoval(fulfillableUnits);
            }
            if (unfulfillableUnits > 0)
            {
                inventory.LabelUnfulfillableUnitsForRemoval(unfulfillableUnits);
            }
        });
    }

    #region Helpers
    private Result UpdateStockAndPublishEvent(
        string productId, uint units, Action<ProductInventory> update)
    {
        if (units == 0)
        {
            return Result.Invalid(new ValidationError()
            {
                Identifier = nameof(units),
                ErrorMessage = "Number of units must be larger than 0."
            });
        }

        var productInventory = _productInventoryRepo.GetByProductId(productId);
        if (productInventory == null)
        {
            return Result.NotFound("Inventory not found for product ID.");
        }

        try
        {
            update(productInventory);
        }
        catch (Exception ex) when (ex is NotEnoughStockException
            || ex is ExceedingMaxStockThresholdException)
        {
            return Result.Conflict(ex.Message);
        }

        _productInventoryRepo.Update(productInventory);
        _eventBus.Publish(
            new ProductFbbInventoryUpdatedIntegrationEvent(
                productId, productInventory.FulfillableUnitsInStock,
                productInventory.UnfulfillableUnitsInStock,
                productInventory.FulfillableUnitsPendingRemoval,
                productInventory.UnfulfillableUnitsPendingRemoval));
        return Result.Success();
    }
    #endregion
}

namespace Bazaar.FbbInventory.Domain.Services;

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

    public Result ReduceStock(string productId, uint units)
    {
        if (units == 0)
        {
            return Result.Invalid(new ValidationError()
            {
                Identifier = nameof(units),
                ErrorMessage = "Number of stock units to reduce must be larger than 0."
            });
        }

        var productInventory = _productInventoryRepo.GetByProductId(productId);
        if (productInventory == null)
        {
            return Result.NotFound("Product not found.");
        }

        try
        {
            productInventory.ReduceStock(units);
        }
        catch (NotEnoughStockException ex)
        {
            return Result.Conflict(ex.Message);
        }

        _productInventoryRepo.Update(productInventory);
        _eventBus.Publish(
            new ProductFbbInventoryUpdatedIntegrationEvent(
                productId, productInventory.UnitsInStock));
        return Result.Success();
    }

    public Result Restock(string productId, uint units)
    {
        if (units == 0)
        {
            return Result.Invalid(new ValidationError()
            {
                Identifier = nameof(units),
                ErrorMessage = "Number of stock units to restock must be larger than 0."
            });
        }

        var productInventory = _productInventoryRepo.GetByProductId(productId);
        if (productInventory == null)
        {
            return Result.NotFound("Product not found.");
        }

        try
        {
            productInventory.Restock(units);
        }
        catch (ExceedingMaxStockThresholdException ex)
        {
            return Result.Conflict(ex.Message);
        }

        _productInventoryRepo.Update(productInventory);
        _eventBus.Publish(
            new ProductFbbInventoryUpdatedIntegrationEvent(
                productId, productInventory.UnitsInStock));
        return Result.Success();
    }
}

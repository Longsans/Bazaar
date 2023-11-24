namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface IUpdateProductStockService
{
    Result ReduceStock(string productId,
        uint fulfillableUnits, uint unfulfillableUnits);

    Result AddFulfillableStock(string productId, uint units);

    Result AddUnfulfillableStock(string productId,
        UnfulfillableCategory category, uint units);

    Result LabelStockUnitsForRemoval(string productId,
        uint fulfillableUnits, uint unfulfillableUnits);
}

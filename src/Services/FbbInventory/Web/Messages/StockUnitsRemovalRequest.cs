namespace Bazaar.FbbInventory.Web.Messages;

public readonly record struct StockUnitsRemovalRequest(
    IEnumerable<StockUnitsRemovalItem> RemovalQuantities,
    RemovalMethod RemovalMethod, string? DeliveryAddress);

public readonly record struct StockUnitsRemovalItem(string ProductId,
    uint FulfillableUnits, uint UnfulfillableUnits);
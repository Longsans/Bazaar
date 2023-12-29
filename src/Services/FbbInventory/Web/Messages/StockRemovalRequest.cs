namespace Bazaar.FbbInventory.Web.Messages;

public readonly record struct StockRemovalRequest(
    IEnumerable<StockRemovalRequestItem> RemovalQuantities,
    RemovalMethod RemovalMethod, string? DeliveryAddress);

public readonly record struct StockRemovalRequestItem(string ProductId,
    uint GoodQuantity, uint UnfulfillableQuantity);

public enum RemovalMethod
{
    Return = 1,
    Disposal = 2,
}
namespace Bazaar.FbbInventory.Web.Messages;

public record LotsRemovalRequest(
    IEnumerable<string> LotNumbers,
    RemovalMethod RemovalMethod,
    string? DeliveryAddress);

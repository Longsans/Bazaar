namespace Bazaar.FbbInventory.Web.Messages;

public record AddUnfulfillableStockRequest(
    UnfulfillableCategory UnfulfillableCategory,
    uint Units);

namespace Bazaar.FbbInventory.Web.Messages;

public record ReduceProductStockRequest(uint FulfillableUnits, uint UnfulfillableUnits);

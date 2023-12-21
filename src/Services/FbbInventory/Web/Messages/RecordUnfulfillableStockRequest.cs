namespace Bazaar.FbbInventory.Web.Messages;

public record RecordUnfulfillableStockRequest(uint Quantity, UnfulfillableCategory UnfulfillableCategory);

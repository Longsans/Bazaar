namespace Bazaar.Catalog.Web.Messages;

public record UpdateStockRequest(string ProductId, uint Units);

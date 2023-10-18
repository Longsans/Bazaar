namespace Bazaar.Catalog.Web.Messages;

public record UpdateStockThresholdsRequest(uint RestockThreshold, uint MaxStockThreshold);

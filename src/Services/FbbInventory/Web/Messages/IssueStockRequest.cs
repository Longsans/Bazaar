namespace Bazaar.FbbInventory.Web.Messages;

public record IssueStockRequest(
    IEnumerable<OutboundStockQuantity> IssueQuantities,
    StockIssueReason IssueReason);

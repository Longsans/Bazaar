namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record StockIssuedIntegrationEvent : IntegrationEvent
{
    public DateTime DateOfIssue { get; }
    public StockIssueReason IssueReason { get; }
    public IEnumerable<IssuedStockEventItem> Items { get; }

    public StockIssuedIntegrationEvent(DateTime dateOfIssue,
        StockIssueReason issueReason, IEnumerable<IssuedStockEventItem> items)
    {
        DateOfIssue = dateOfIssue;
        IssueReason = issueReason;
        Items = items;
    }
}

public record IssuedStockEventItem
{
    public string ProductId { get; }
    public uint GoodQuantity { get; }
    public uint UnfulfillableQuantity { get; }

    public IssuedStockEventItem(string productId, uint goodQuantity, uint unfulfillableQuantity)
    {
        ProductId = productId;
        GoodQuantity = goodQuantity;
        UnfulfillableQuantity = unfulfillableQuantity;
    }
}
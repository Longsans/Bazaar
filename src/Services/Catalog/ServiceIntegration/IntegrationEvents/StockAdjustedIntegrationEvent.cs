namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record StockAdjustedIntegrationEvent : IntegrationEvent
{
    public DateTime DateOfRestoration { get; }
    public IEnumerable<AdjustedQuantity> QuantitiesAdjusted { get; }

    public StockAdjustedIntegrationEvent(
        DateTime dateOfRestoration, IEnumerable<AdjustedQuantity> quantitiesAdjusted)
    {
        DateOfRestoration = dateOfRestoration.Date;
        QuantitiesAdjusted = quantitiesAdjusted;
    }
}

public record AdjustedQuantity
{
    public string ProductId { get; }
    public int GoodQuantity { get; }
    public int UnfulfillableQuantity { get; }
    public bool IsStrandedStatus { get; }

    public AdjustedQuantity(string productId, int goodQuantity, int unfulfillableQuantity, bool isStrandedStatus)
    {
        ProductId = productId;
        GoodQuantity = goodQuantity;
        UnfulfillableQuantity = unfulfillableQuantity;
        IsStrandedStatus = isStrandedStatus;
    }
}

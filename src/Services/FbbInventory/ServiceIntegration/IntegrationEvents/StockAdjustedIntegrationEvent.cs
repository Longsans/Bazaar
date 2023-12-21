namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record StockAdjustedIntegrationEvent : IntegrationEvent
{
    public DateTime DateOfRestoration { get; }
    public IEnumerable<AdjustedQuantity> QuantitiesAdjusted { get; }

    public StockAdjustedIntegrationEvent(
        DateTime dateOfRestoration, IEnumerable<AdjustedQuantity> items)
    {
        DateOfRestoration = dateOfRestoration.Date;
        QuantitiesAdjusted = items;
    }
}

public record AdjustedQuantity(string ProductId, int GoodQuantity, int UnfulfillableQuantity, bool IsStrandedStatus)
{
    public AdjustedQuantity(string productId, uint goodQuantity, uint unfulfillableQuantity, bool isStrandedStatus)
        : this(productId, (int)goodQuantity, (int)unfulfillableQuantity, isStrandedStatus)
    {

    }
}

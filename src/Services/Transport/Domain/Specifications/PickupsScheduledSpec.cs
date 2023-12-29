namespace Bazaar.Transport.Domain.Specifications;

public class PickupsScheduledSpec : Specification<InventoryPickup>
{
    public PickupsScheduledSpec()
    {
        Query.Include(x => x.ProductStocks)
            .Where(x => x.Status == InventoryPickupStatus.Scheduled);
    }

    public PickupsScheduledSpec(string productId) : this()
    {
        Query.PostProcessingAction(pickups =>
            pickups.Where(x => PickupPredicates.ContainsProduct(x, productId)));
    }
}

namespace Bazaar.Transport.Domain.Specifications;

public class PickupsCompletedSpec : Specification<InventoryPickup>
{
    public PickupsCompletedSpec()
    {
        Query.Include(x => x.ProductStocks)
            .Where(x => x.Status == InventoryPickupStatus.Completed);
    }

    public PickupsCompletedSpec(string productId) : this()
    {
        Query.PostProcessingAction(pickups =>
            pickups.Where(x => PickupPredicates.ContainsProduct(x, productId)));
    }
}

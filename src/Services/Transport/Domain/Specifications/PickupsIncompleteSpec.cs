namespace Bazaar.Transport.Domain.Specifications;

public class PickupsIncompleteSpec : Specification<InventoryPickup>
{
    public PickupsIncompleteSpec()
    {
        Query.Include(x => x.ProductStocks)
            .Where(x => x.Status != InventoryPickupStatus.Completed
                && x.Status != InventoryPickupStatus.Cancelled);
    }

    public PickupsIncompleteSpec(string productId) : this()
    {
        Query.PostProcessingAction(pickups =>
            pickups.Where(x => PickupPredicates.ContainsProduct(x, productId)));
    }
}

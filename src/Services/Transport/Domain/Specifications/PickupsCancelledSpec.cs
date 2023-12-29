namespace Bazaar.Transport.Domain.Specifications;

public class PickupsCancelledSpec : Specification<InventoryPickup>
{
    public PickupsCancelledSpec()
    {
        Query.Include(x => x.ProductStocks)
            .Where(x => x.Status == InventoryPickupStatus.Cancelled);
    }

    public PickupsCancelledSpec(string productId) : this()
    {
        Query.PostProcessingAction(pickups =>
            pickups.Where(x => PickupPredicates.ContainsProduct(x, productId)));
    }
}

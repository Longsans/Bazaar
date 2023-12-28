namespace Bazaar.Transport.Domain.Specifications;

public class PickupsInProgressSpec : Specification<InventoryPickup>
{
    public PickupsInProgressSpec()
    {
        Query.Include(x => x.ProductStocks)
            .Where(x => x.Status == InventoryPickupStatus.EnRouteToPickupLocation
                || x.Status == InventoryPickupStatus.DeliveringToWarehouse);
    }

    public PickupsInProgressSpec(string productId) : this()
    {
        Query.PostProcessingAction(pickups =>
            pickups.Where(x => PickupPredicates.ContainsProduct(x, productId)));
    }
}

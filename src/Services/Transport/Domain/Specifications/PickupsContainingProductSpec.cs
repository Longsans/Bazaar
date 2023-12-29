namespace Bazaar.Transport.Domain.Specifications;

public class PickupsContainingProductSpec : Specification<InventoryPickup>
{
    public PickupsContainingProductSpec(string productId)
    {
        Query.Include(x => x.ProductStocks)
            .PostProcessingAction(pickups => pickups.Where(
                x => PickupPredicates.ContainsProduct(x, productId)));
    }
}

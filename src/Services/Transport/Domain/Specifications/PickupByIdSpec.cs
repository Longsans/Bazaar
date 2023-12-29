namespace Bazaar.Transport.Domain.Specifications;

public class PickupByIdSpec : SingleResultSpecification<InventoryPickup>
{
    public PickupByIdSpec(int id)
    {
        Query.Include(x => x.ProductStocks)
            .Where(x => x.Id == id);
    }
}

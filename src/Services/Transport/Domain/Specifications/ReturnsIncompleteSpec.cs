namespace Bazaar.Transport.Domain.Specifications;

public class ReturnsIncompleteSpec : Specification<InventoryReturn>
{
    public ReturnsIncompleteSpec()
    {
        Query.Include(x => x.ReturnQuantities)
            .Where(x => x.Status == DeliveryStatus.Scheduled
                || x.Status == DeliveryStatus.Delivering);
    }
}

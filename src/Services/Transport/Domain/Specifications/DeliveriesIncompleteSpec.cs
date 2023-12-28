namespace Bazaar.Transport.Domain.Specifications;

public class DeliveriesIncompleteSpec : Specification<Delivery>
{
    public DeliveriesIncompleteSpec()
    {
        Query.Include(x => x.PackageItems)
            .Where(x => x.Status == DeliveryStatus.Scheduled
                || x.Status == DeliveryStatus.Delivering);
    }
}

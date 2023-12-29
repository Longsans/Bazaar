namespace Bazaar.Transport.Domain.Specifications;

public class DeliveryByIdSpec : SingleResultSpecification<Delivery>
{
    public DeliveryByIdSpec(int id)
    {
        Query.Include(x => x.PackageItems)
            .Where(x => x.Id == id);
    }
}

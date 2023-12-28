namespace Bazaar.Transport.Domain.Specifications;

public class ReturnByIdSpec : SingleResultSpecification<InventoryReturn>
{
    public ReturnByIdSpec(int id)
    {
        Query.Include(x => x.ReturnQuantities)
            .Where(x => x.Id == id);
    }
}

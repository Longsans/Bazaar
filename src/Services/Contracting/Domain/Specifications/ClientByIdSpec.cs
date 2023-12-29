namespace Bazaar.Contracting.Domain.Specifications;

public class ClientByIdSpec : ClientWithContractsAndPlanSpec
{
    public ClientByIdSpec(int id, bool includeClosedAccount = false)
        : base(includeClosedAccount)
    {
        Query.Where(x => x.Id == id);
    }
}

namespace Bazaar.Contracting.Domain.Specifications;

public class ClientWithContractsAndPlanSpec : SingleResultSpecification<Client>
{
    public ClientWithContractsAndPlanSpec(bool includeClosedAccount = false)
    {
        Query.Include(x => x.Contracts)
            .Include(x => x.SellingPlan);

        if (!includeClosedAccount)
            Query.Where(x => !x.IsAccountClosed);
    }
}

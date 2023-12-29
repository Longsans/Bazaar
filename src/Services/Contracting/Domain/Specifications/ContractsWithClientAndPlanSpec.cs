namespace Bazaar.Contracting.Domain.Specifications;

public class ContractsWithClientAndPlanSpec : Specification<Contract>
{
    public ContractsWithClientAndPlanSpec()
    {
        Query.Include(x => x.Client)
            .Include(x => x.SellingPlan);
    }
}

namespace Bazaar.Contracting.Domain.Specifications;

public class ContractByIdSpec : ContractsWithClientAndPlanSpec, ISingleResultSpecification<Contract>
{
    public ContractByIdSpec(int id)
    {
        Query.Where(x => x.Id == id);
    }
}

namespace Bazaar.Contracting.Domain.Specifications;

public class ContractsByClientExternalIdSpec : ContractsWithClientAndPlanSpec
{
    public ContractsByClientExternalIdSpec(string clientExternalId)
    {
        Query.Where(x => x.Client.ExternalId == clientExternalId);
    }
}

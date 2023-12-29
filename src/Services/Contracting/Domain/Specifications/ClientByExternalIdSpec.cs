namespace Bazaar.Contracting.Domain.Specifications;

public class ClientByExternalIdSpec : ClientWithContractsAndPlanSpec
{
    public ClientByExternalIdSpec(string externalId, bool includeClosedAccount = false)
        : base(includeClosedAccount)
    {
        Query.Where(x => x.ExternalId == externalId);
    }
}

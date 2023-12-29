namespace Bazaar.Contracting.Domain.Specifications;

public class ClientByEmailAddressSpec : ClientWithContractsAndPlanSpec
{
    public ClientByEmailAddressSpec(string emailAddress, bool includeClosedAccount = false)
        : base(includeClosedAccount)
    {
        Query.Where(x => x.EmailAddress == emailAddress);
    }
}

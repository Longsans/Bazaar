namespace Bazaar.Contracting.Web.Messages;

public class PartnerQuery
{
    public int Id { get; set; }
    public string ExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public List<ContractQuery> Contracts { get; set; }

    public PartnerQuery(Partner partner)
    {
        Id = partner.Id;
        ExternalId = partner.ExternalId;
        FirstName = partner.FirstName;
        LastName = partner.LastName;
        Email = partner.EmailAddress;
        PhoneNumber = partner.PhoneNumber;
        Contracts = partner.Contracts.Select(c => new ContractQuery(c)).ToList();
    }
}
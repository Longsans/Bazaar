namespace Bazaar.Contracting.Web.Messages;

public class ClientResponse
{
    public int Id { get; set; }
    public string ExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public List<ContractResponse> Contracts { get; set; }

    public ClientResponse(Client client)
    {
        Id = client.Id;
        ExternalId = client.ExternalId;
        FirstName = client.FirstName;
        LastName = client.LastName;
        Email = client.EmailAddress;
        PhoneNumber = client.PhoneNumber;
        Contracts = client.Contracts.Select(c => new ContractResponse(c)).ToList();
    }
}
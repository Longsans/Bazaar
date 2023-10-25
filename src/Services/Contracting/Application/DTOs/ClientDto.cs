namespace Bazaar.Contracting.Application.DTOs;

public class ClientDto
{
    public int? Id { get; set; }
    public string? ExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }

    public ClientDto() { }

    public ClientDto(Client client)
    {
        Id = client.Id;
        FirstName = client.FirstName;
        LastName = client.LastName;
        EmailAddress = client.EmailAddress;
        PhoneNumber = client.PhoneNumber;
        DateOfBirth = client.DateOfBirth;
        Gender = client.Gender;
    }

    public Client ToNewClient()
    {
        return new Client(
            FirstName,
            LastName,
            EmailAddress,
            PhoneNumber,
            DateOfBirth,
            Gender);
    }

    public Client ToExistingClient(int id, string externalId)
    {
        Id ??= id;
        ExternalId ??= externalId;

        return new Client(
            Id.Value,
            ExternalId,
            FirstName,
            LastName,
            EmailAddress,
            PhoneNumber,
            DateOfBirth,
            Gender);
    }
}

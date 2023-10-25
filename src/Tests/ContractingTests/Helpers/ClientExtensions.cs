namespace ContractingTests.Helpers;

public static class ClientExtensions
{
    public static Client Clone(this Client client)
    {
        return new Client(
            client.FirstName, client.LastName, client.EmailAddress,
            client.PhoneNumber, client.DateOfBirth, client.Gender);
    }

    public static Client WithDifferentId(this Client client, int id, string externalId)
    {
        return new Client(id, externalId,
            client.FirstName, client.LastName, client.EmailAddress,
            client.PhoneNumber, client.DateOfBirth, client.Gender);
    }

    public static Client WithEmailAddress(this Client client, string emailAddress)
    {
        var clone = client.Clone();

        typeof(Client).GetProperty(nameof(clone.EmailAddress))
            .SetValue(clone, emailAddress, null);

        return clone;
    }
}

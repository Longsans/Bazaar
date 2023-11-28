namespace ContractingTests.Helpers;

public static class ClientExtensions
{
    public static Client Clone(this Client client)
    {
        return new Client(
            client.FirstName, client.LastName, client.EmailAddress,
            client.PhoneNumber, client.DateOfBirth, client.Gender, client.SellingPlanId);
    }

    public static Client WithDifferentId(this Client client, int id, string externalId)
    {
        var clone = client.Clone();

        typeof(Client).GetProperty(nameof(clone.Id))
            .SetValue(clone, id, null);
        typeof(Client).GetProperty(nameof(clone.ExternalId))
            .SetValue(clone, externalId, null);

        return clone;
    }

    public static Client WithEmailAddress(this Client client, string emailAddress)
    {
        var clone = client.Clone();

        typeof(Client).GetProperty(nameof(clone.EmailAddress))
            .SetValue(clone, emailAddress, null);

        return clone;
    }
}

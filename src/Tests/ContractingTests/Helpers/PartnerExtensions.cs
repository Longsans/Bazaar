namespace ContractingTests.Helpers;

public static class PartnerExtensions
{
    public static Partner Clone(this Partner partner)
    {
        return new Partner(
            partner.FirstName, partner.LastName, partner.EmailAddress,
            partner.PhoneNumber, partner.DateOfBirth, partner.Gender);
    }

    public static Partner WithEmailAddress(this Partner partner, string emailAddress)
    {
        var clone = partner.Clone();

        typeof(Partner).GetProperty(nameof(clone.EmailAddress))
            .SetValue(clone, emailAddress, null);

        return clone;
    }
}

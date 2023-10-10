namespace ContractingTests.Helpers;

public static class PartnerExtensionsAndHelpers
{
    public static readonly Partner ValidNewPartner = new()
    {
        FirstName = "TestFName",
        LastName = "TestLName",
        Email = "Test@testmail.com",
        PhoneNumber = "0123456789",
        DateOfBirth = new DateTime(1989, 11, 10),
        Gender = Gender.Male
    };

    public static readonly Partner ExistingPartner
        = ValidNewPartner.WithValidId();

    public static Partner Clone(this Partner partner)
    {
        return new()
        {
            Id = partner.Id,
            ExternalId = partner.ExternalId,
            FirstName = partner.FirstName,
            LastName = partner.LastName,
            Email = partner.Email,
            PhoneNumber = partner.PhoneNumber,
            DateOfBirth = partner.DateOfBirth,
            Gender = partner.Gender,
            Contracts = partner.Contracts,
        };
    }

    public static Partner WithValidId(this Partner partner)
    {
        var clone = partner.Clone();
        clone.Id = 1;
        clone.ExternalId = "PNER-1";

        return clone;
    }

    public static Partner WithInvalidId(this Partner partner)
    {
        var clone = partner.Clone();
        clone.Id = 123;
        clone.ExternalId = "PNER-123";

        return clone;
    }

    public static Partner WithUnderEighteenAge(this Partner partner)
    {
        var clone = partner.Clone();
        clone.DateOfBirth = new DateTime(
            DateTime.Now.Year - 17,
            partner.DateOfBirth.Month,
            partner.DateOfBirth.Day);

        return clone;
    }

    public static Partner WithUsedEmail(this Partner partner)
    {
        var clone = partner.Clone();
        clone.Email = "Test@testmail.com";
        return clone;
    }

    public static Partner WithChangedInfo(this Partner partner, bool changeEmail = false)
    {
        var clone = partner.Clone();
        clone.FirstName = "Jim";
        clone.LastName = "Jones";
        clone.PhoneNumber = "0123123123";
        clone.DateOfBirth = new DateTime(1999, 10, 10);

        if (changeEmail)
            clone.Email = "Test123@testmail.com";

        return clone;
    }

    public static Partner WithContracts(this Partner partner, params Contract[] contracts)
    {
        var clone = partner.Clone();
        clone.Contracts = contracts.ToList();
        return clone;
    }
}

namespace Bazaar.Contracting.DTOs;

public class PartnerWriteCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }

    public Partner ToPartnerInfo() => new()
    {
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        PhoneNumber = PhoneNumber,
        DateOfBirth = DateOfBirth,
        Gender = Gender,
    };

    public Partner ToPartner(string externalId) => new()
    {
        ExternalId = externalId,
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        PhoneNumber = PhoneNumber,
        DateOfBirth = DateOfBirth,
        Gender = Gender,
    };
}

namespace Bazaar.Contracting.Application.DTOs;

public class PartnerDto
{
    public int? Id { get; set; }
    public string? ExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }

    public PartnerDto() { }

    public PartnerDto(Partner partner)
    {
        Id = partner.Id;
        FirstName = partner.FirstName;
        LastName = partner.LastName;
        Email = partner.EmailAddress;
        PhoneNumber = partner.PhoneNumber;
        DateOfBirth = partner.DateOfBirth;
        Gender = partner.Gender;
    }

    public Partner ToNewPartner()
    {
        return new Partner(
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            DateOfBirth,
            Gender);
    }

    public Partner ToExistingPartner()
    {
        if (Id is null || string.IsNullOrWhiteSpace(ExternalId))
            throw new NullReferenceException("Id and ExternalId must not be null.");

        return new Partner(
            Id.Value,
            ExternalId,
            FirstName,
            LastName,
            Email,
            PhoneNumber,
            DateOfBirth,
            Gender);
    }
}

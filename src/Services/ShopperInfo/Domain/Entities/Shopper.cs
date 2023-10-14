namespace Bazaar.ShopperInfo.Domain.Entities;

public class Shopper
{
    public int Id { get; set; }
    public string ExternalId { get; set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string EmailAddress { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }

    public Shopper(
        string firstName, string lastName,
        string emailAddress, string phoneNumber,
        DateTime dateOfBirth, Gender gender)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Gender = gender;
    }
}

public enum Gender
{
    Male,
    Female,
    NonBinary,
}

namespace Bazaar.ShopperInfo.Domain.Entities;

public class Shopper
{
    public int Id { get; private set; }
    public string ExternalId { get; private set; }
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
        AssertPersonalInfo(firstName, lastName, emailAddress, phoneNumber, dateOfBirth, gender);
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth.Date;
        Gender = gender;
    }

    public void UpdatePersonalInfo(
        string firstName, string lastName, string emailAddress,
        string phoneNumber, DateTime dob, Gender gender)
    {
        AssertPersonalInfo(firstName, lastName, emailAddress, phoneNumber, dob, gender);
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        PhoneNumber = phoneNumber;
        DateOfBirth = dob;
        Gender = gender;
    }

    private static void AssertPersonalInfo(
        string firstName, string lastName, string emailAddress,
        string phoneNumber, DateTime dob, Gender gender)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            throw new ArgumentException("Email address cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("Phone number cannot be empty.");
        }
        if (dob.Date > DateTime.Now.Date)
        {
            throw new ArgumentException("Date of birth cannot be a future date.");
        }
        if (!Enum.IsDefined(gender))
        {
            throw new ArgumentException("Gender value does not exist.");
        }
    }
}

public enum Gender
{
    Male,
    Female,
    NonBinary,
}

namespace WebShoppingUI.Model;

public class Seller
{
    public string Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }
    public string PhoneNumber { get; }
    public DateTime DateOfBirth { get; }
    public Gender Gender { get; }

    public Seller(string id, string firstName, string lastName, string email, string phoneNumber, DateTime dateOfBirth, Gender gender)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Gender = gender;
    }
}

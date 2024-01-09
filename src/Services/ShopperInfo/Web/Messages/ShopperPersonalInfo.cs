namespace ShopperInfo.Web.Messages;

public record ShopperPersonalInfo
{
    public string FirstName { get; }
    public string LastName { get; }
    public string PhoneNumber { get; }
    public DateTime DateOfBirth { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; }

    public ShopperPersonalInfo(string firstName, string lastName, string phoneNumber, DateTime dateOfBirth, Gender gender)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Gender = gender;
    }
}

namespace Bazaar.ShopperInfo.Web.Messages;

public class ShopperWriteRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; set; }

    public Shopper ToExistingShopper(string externalId)
    {
        return new Shopper(
            FirstName, LastName, Email,
            PhoneNumber, DateOfBirth, Gender)
        {
            ExternalId = externalId
        };
    }

    public Shopper ToNewShopper()
    {
        return new Shopper(
            FirstName, LastName, Email,
            PhoneNumber, DateOfBirth, Gender);
    }
}

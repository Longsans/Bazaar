namespace Bazaar.ShopperInfo.DTOs;

public class ShopperWriteCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; set; }

    public Shopper ToShopperInfo() => new()
    {
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        PhoneNumber = PhoneNumber,
        DateOfBirth = DateOfBirth,
        Gender = Gender,
    };
}

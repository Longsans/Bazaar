namespace WebShoppingUI.Model;

using System.Text.Json.Serialization;

public class ShopperWriteCommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }

    [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; set; }
}

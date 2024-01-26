namespace WebShoppingUI.DTOs;

public class ShopperQuery
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string DateOfBirth { get; set; }
    public string Gender { get; set; }

    public ShopperQuery(Shopper shopper)
    {
        Id = shopper.ExternalId;
        FirstName = shopper.FirstName;
        LastName = shopper.LastName;
        EmailAddress = shopper.EmailAddress;
        PhoneNumber = shopper.PhoneNumber;
        DateOfBirth = shopper.DateOfBirth.ToString("yyyy-MM-dd");
        Gender = shopper.Gender switch
        {
            Model.Gender.NonBinary => "Non-binary",
            var g => g.ToString()
        };
    }
}

namespace WebShoppingUI.DTOs;

public class ShopperQuery
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string DateOfBirth { get; set; }
    public string Gender { get; set; }

    public ShopperQuery(Shopper shopper)
    {
        Id = shopper.ExternalId;
        FirstName = shopper.FirstName;
        LastName = shopper.LastName;
        Email = shopper.Email;
        PhoneNumber = shopper.PhoneNumber;
        DateOfBirth = shopper.DateOfBirth.ToString("dd/MM/yyyy");
        Gender = shopper.Gender switch
        {
            Model.Gender.NonBinary => "Non-binary",
            var g => g.ToString()
        };
    }
}

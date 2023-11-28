namespace Bazaar.Contracting.Web.Messages;

public class RegisterClientRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public int SellingPlanId { get; set; }
}

namespace Bazaar.Contracting.Web.Messages;

public record UpdateClientInfoRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
}

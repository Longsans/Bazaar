namespace Bazaar.Identity.Model;

public class ApplicationUserToken : IdentityUserToken<string>
{
    public int Id { get; set; }
}

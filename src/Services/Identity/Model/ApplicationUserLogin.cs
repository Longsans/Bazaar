namespace Bazaar.Identity.Model;

public class ApplicationUserLogin : IdentityUserLogin<string>
{
    public int Id { get; set; }
}

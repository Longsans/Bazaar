namespace Bazaar.Identity.Model;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

    public ApplicationUser() { }
    public ApplicationUser(string email) : base(email)
    {
        Email = email;
    }
}

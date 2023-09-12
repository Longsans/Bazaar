namespace Bazaar.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    string,
    IdentityUserClaim<string>,
    ApplicationUserRole,
    ApplicationUserLogin,
    IdentityRoleClaim<string>,
    ApplicationUserToken>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(user =>
        {
            user.HasMany(u => u.UserRoles)
                .WithOne()
                .HasForeignKey(u => u.UserId)
                .IsRequired();

            user.Property(u => u.Email)
                .IsRequired();

            user.HasIndex(u => u.Email)
                .IsUnique();
        });

        builder.Entity<ApplicationRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne()
            .HasForeignKey(u => u.RoleId)
            .IsRequired();

        builder.Entity<ApplicationUserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
    }
}

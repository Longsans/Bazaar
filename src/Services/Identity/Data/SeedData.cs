namespace Identity.Data;

public static class SeedData
{
    public async static Task<WebApplication> SeedDatabase(this WebApplication app)
    {
        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.EnsureDeleted();
        await dbContext.Database.MigrateAsync();

        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var roles = new string[] { "Shopper", "Seller" };
        foreach (var role in roles)
        {
            var result = await roleMgr.CreateAsync(new()
            {
                Name = role
            });
            if (!result.Succeeded)
            {
                throw new Exception($"Role {role} was not successfully created.");
            }
        }

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var userEmails = new string[] { "rando@gmail.com", "jbezos@amazon.com" };
        var userPwds = new string[] { "R@nd00", "B$z0ss" };
        foreach (var (userEmail, index) in userEmails.Select((item, index) => (item, index)))
        {
            var user = new ApplicationUser(userEmail);
            var result = await userMgr.CreateAsync(user, userPwds[index]);
            if (!result.Succeeded)
            {
                throw new Exception($"User {userEmail} was not successfully created.");
            }

            result = await userMgr.AddToRoleAsync(user, roles[index]);
            if (!result.Succeeded)
            {
                throw new Exception($"User {userEmail} was not successfully added to role {roles[index]}.");
            }
        }

        return app;
    }
}

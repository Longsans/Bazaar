namespace Bazaar.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new("catalog", "Catalog API")
            {
                Scopes = { "catalog.read", "catalog.modify" }
            },
            new("basket", "Basket API")
            {
                Scopes = { "basket" }
            },
            new("ordering", "Ordering API")
            {
                Scopes = { "ordering" }
            },
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new()
            {
                ClientId = "webshopping",
                ClientSecrets = { new Secret("webshoppingsecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "catalog.read",
                    "basket",
                    "ordering",
                },
                AllowOfflineAccess = true,
                RedirectUris = { "https://localhost:44434/signin-oidc" }
            },
            new()
            {
                ClientId = "webseller",
                ClientSecrets = { new Secret("websellersecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "catalog.read",
                    "catalog.modify",
                    "ordering",
                },
                AllowOfflineAccess = true,
                RedirectUris = { "https://localhost:7265/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:7265/signout-callback-oidc" }
            }
        };
}
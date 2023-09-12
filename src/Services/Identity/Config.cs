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
            new("contracting", "Contracting API")
            {
                Scopes = { "contracting" }
            },
            new("shopper_info", "Shopper info API")
            {
                Scopes = { "shopper_info" }
            },
        };

    public static IEnumerable<Client> Clients(IConfiguration config) =>
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
                    "shopper_info"
                },
                AllowOfflineAccess = true,
                RedirectUris =
                {
                    $"{config["WebShoppingRedirectUrl"]}/signin-oidc"
                },
                PostLogoutRedirectUris =
                {
                    $"{config["WebShoppingRedirectUrl"]}/signout-callback-oidc"
                }
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
                    "contracting",
                },
                AllowOfflineAccess = true,
                RedirectUris =
                {
                    $"{config["WebSellerRedirectUrl"]}/signin-oidc"
                },
                PostLogoutRedirectUris =
                {
                    $"{config["WebSellerRedirectUrl"]}/signout-callback-oidc"
                }
            }
        };
}
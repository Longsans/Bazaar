using Duende.Bff.Yarp;
using System.IdentityModel.Tokens.Jwt;
using WebSellerUI.HttpHandlers;
using WebSellerUI.Managers;

namespace WebSellerUI.Pipeline;

public static class ConfigExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy => policy.AllowAnyOrigin().AllowAnyHeader());
        });

        // IdentityServer BFF
        builder.Services.AddBff()
            .AddRemoteApis();

        // Authentication
        JwtSecurityTokenHandler.DefaultMapInboundClaims = true;
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookie";
            options.DefaultChallengeScheme = "oidc";
            options.DefaultSignOutScheme = "oidc";
        })
            .AddCookie("Cookie")
            .AddOpenIdConnect("oidc", options =>
            {
                options.RequireHttpsMetadata = false;

                options.Authority = builder.Configuration["IdentityApi"];
                options.ClientId = builder.Configuration["ClientId"];
                options.ClientSecret = builder.Configuration["ClientSecret"];
                options.ResponseType = "code";
                options.ResponseMode = "query";

                options.Scope.Add("catalog.read");
                options.Scope.Add("catalog.modify");
                options.Scope.Add("ordering");
                options.Scope.Add("contracting");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
            });

        // Authorization
        builder.Services.AddAuthorization();

        // HTTP clients and services
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<HttpClientAuthorizationHandler>();

        builder.Services.AddHttpClient<ICatalogDataService, HttpCatalogService>()
            .AddHttpMessageHandler<HttpClientAuthorizationHandler>();

        builder.Services.AddHttpClient<IOrderingDataService, HttpOrderingService>()
            .AddHttpMessageHandler<HttpClientAuthorizationHandler>();

        builder.Services.AddHttpClient<IContractingDataService, HttpContractingService>()
            .AddHttpMessageHandler<HttpClientAuthorizationHandler>();

        builder.Services.AddScoped<AddressService>();

        // Business logic services
        builder.Services.AddTransient<OrderManager>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseIdentityServerBrowserAddresses();

        app.UseCors();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();

        app.UseBff();
        app.UseAuthorization();

        app.MapBffManagementEndpoints();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}")
            .RequireAuthorization();

        app.MapFallbackToFile("index.html");

        return app;
    }
}

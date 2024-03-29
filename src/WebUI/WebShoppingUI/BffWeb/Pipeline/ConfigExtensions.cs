﻿using Duende.Bff.Yarp;
using System.IdentityModel.Tokens.Jwt;
using WebShoppingUI.HttpHandlers;

namespace WebShoppingUI.Pipeline;

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

        // AuthN
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
                options.Scope.Add("basket");
                options.Scope.Add("ordering");
                options.Scope.Add("shopper_info");

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
            });

        // AuthZ
        builder.Services.AddAuthorization();

        // HTTP services
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<ApiEndpointResolver>();

        builder.Services.AddTransient<HttpClientAuthorizationHandler>();

        builder.Services.AddHttpClient<IShopperInfoDataService, HttpShopperInfoService>()
            .AddHttpMessageHandler<HttpClientAuthorizationHandler>();

        builder.Services.AddHttpClient<ICatalogDataService, HttpCatalogService>()
            .AddHttpMessageHandler<HttpClientAuthorizationHandler>();

        builder.Services.AddHttpClient<IBasketDataService, HttpBasketService>()
            .AddHttpMessageHandler<HttpClientAuthorizationHandler>();

        builder.Services.AddHttpClient<IOrderingDataService, HttpOrderingService>()
            .AddHttpMessageHandler<HttpClientAuthorizationHandler>();

        builder.Services.AddHttpClient<IContractingService, HttpContractingService>()
            .AddHttpMessageHandler<HttpClientAuthorizationHandler>();

        // Business logic services
        builder.Services.AddTransient<BasketManager>()
            .AddTransient<OrderManager>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        var IF_ENABLED_IDENTITY = (Action doWithIdentity) =>
        {
            if (string.IsNullOrWhiteSpace(app.Configuration["DisableIdentity"]))
                doWithIdentity();
        };

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseIdentityServerBrowserAddresses();

        app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        app.UseStaticFiles();
        app.UseRouting();

        // DISABLES IDENTITY
        IF_ENABLED_IDENTITY(() =>
        {
            app.UseAuthentication();

            app.UseBff();
            app.UseAuthorization();

            app.MapBffManagementEndpoints();
        });

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}")
            .RequireAuthorization();

        app.MapFallbackToFile("index.html");

        return app;
    }
}

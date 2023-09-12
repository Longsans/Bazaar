using Microsoft.Extensions.Primitives;

namespace WebShoppingUI.Middlewares;

public class IdentityServerBrowserAddressesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string IDENTITY_API;
    private readonly string IDENTITY_REWRITE_URL;

    public IdentityServerBrowserAddressesMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        IDENTITY_API = config["IdentityApi"]!;
        IDENTITY_REWRITE_URL = config["IdentityRewriteUrl"]!;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
        if (context.Request.Path == "/bff/login" ||
        context.Request.Path.StartsWithSegments("/bff/logout") &&
        context.Response.StatusCode == 302)
        {
            var correctedEndpoint =
                context.Response.Headers.Location
                    .ToString()
                    .Replace(
                        IDENTITY_API, IDENTITY_REWRITE_URL);
            context.Response.Headers.Location = new StringValues(correctedEndpoint);
        }
    }
}

public static class IdentityServerBrowserAddressesMiddlewareExtensions
{
    public static IApplicationBuilder UseIdentityServerBrowserAddresses(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<IdentityServerBrowserAddressesMiddleware>();
        return builder;
    }
}
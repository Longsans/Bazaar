using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;

namespace WebSellerUI.HttpHandlers;

public class HttpClientAuthorizationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;

    public HttpClientAuthorizationHandler(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var context = _contextAccessor.HttpContext;
        if (context != null)
        {
            var token = await context.GetTokenAsync("access_token");
            if (token != null)
            {
                request.SetBearerToken(token);
            }
        }
        return await base.SendAsync(request, cancellationToken);
    }
}

﻿using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;

namespace WebShoppingUI.Services;

public class HttpService
{
    protected readonly HttpClient _httpClient;
    protected readonly IHttpContextAccessor _ctxAccessor;

    public HttpService(HttpClient httpClient, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _ctxAccessor = contextAccessor;
    }

    protected async Task SetAccessToken()
    {
        var token = await _ctxAccessor.HttpContext.GetTokenAsync("access_token");
        if (token != null)
        {
            _httpClient.SetBearerToken(token);
        }
    }

    protected async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content);
    }
}

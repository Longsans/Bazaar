using Duende.Bff.Yarp;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => policy.AllowAnyOrigin().AllowAnyHeader());
});

builder.Services.AddBff()
    .AddRemoteApis();

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
        options.Authority = builder.Configuration["IdentityUrl"];
        options.ClientId = builder.Configuration["ClientId"];
        options.ClientSecret = builder.Configuration["ClientSecret"];
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.Scope.Add("catalog.read");
        options.Scope.Add("catalog.modify");
        options.Scope.Add("ordering");

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();

app.UseBff();
app.UseAuthorization();
app.MapBffManagementEndpoints();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

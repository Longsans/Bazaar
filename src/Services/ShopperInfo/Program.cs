var builder = WebApplication.CreateBuilder(args);
var IF_IDENTITY_ELSE = (Action doWithIdentity, Action doWithoutIdentity) =>
{
    if (string.IsNullOrWhiteSpace(builder.Configuration["DisableIdentity"]))
    {
        doWithIdentity();
    }
    else
    {
        doWithoutIdentity();
    }
};

// Add services to the container.
#region Register app service
builder.Services.AddScoped<ShopperEmailAddressService>();
builder.Services
    .AddDbContext<ShopperInfoDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration["ConnectionString"]);
    })
    .AddScoped<IShopperRepository, ShopperRepository>()
    .AddScoped(_ => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityApi"];
        options.Audience = "shopper_info";
        options.RequireHttpsMetadata = false;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasShopperInfoScope", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("scope", "shopper_info"));
});
#endregion

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        var enumConverter = new JsonStringEnumConverter();
        options.JsonSerializerOptions.Converters.Add(enumConverter);
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// DISABLES IDENTITY
IF_IDENTITY_ELSE(
    () =>
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers().RequireAuthorization("HasShopperInfoScope");
    },
    () => app.MapControllers()
);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShopperInfoDbContext>();
    await dbContext.Seed(scope.ServiceProvider);
}

app.Run();

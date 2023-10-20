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

#region Register app services
builder.Services.AddDbContext<ContractingDbContext>(options =>
{
    //builder.Configuration["ConnectionString"] =
    //@"Server=.;Database=Bazaar;Trusted_Connection=True;TrustServerCertificate=True;";
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

// Domain services
builder.Services.AddScoped<
    IUpdatePartnerEmailAddressService,
    UpdatePartnerEmailAddressService>();

// Application use-cases
builder.Services.AddScoped<IContractUseCases, ContractUseCases>();
builder.Services.AddScoped<IPartnerUseCases, PartnerUseCases>();
builder.Services.AddScoped<ISellingPlanUseCases, SellingPlanUseCases>();

// Data services
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<ISellingPlanRepository, SellingPlanRepository>();
builder.Services.AddScoped(_ => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));

// AuthN and AuthZ
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityApi"];
        options.Audience = "contracting";
        options.RequireHttpsMetadata = false;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HasContractingScope", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("scope", "contracting"));
});
#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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

        app.MapControllers().RequireAuthorization("HasContractingScope");
    },
    () => app.MapControllers()
);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContractingDbContext>();
    await dbContext.Seed(scope.ServiceProvider);
}

app.Run();

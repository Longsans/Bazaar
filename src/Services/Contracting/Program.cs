using Bazaar.Contracting.Repositories;

var builder = WebApplication.CreateBuilder(args);

#region Register app services
builder.Services.AddDbContext<ContractingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped(_ => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization("HasContractingScope");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContractingDbContext>();
    await dbContext.Seed(scope.ServiceProvider);
}

app.Run();

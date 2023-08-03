using Bazaar.Contracting.Repositories;

var builder = WebApplication.CreateBuilder(args);

#region Register app services
builder.Services.AddDbContext<ContractingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
    //options.UseSqlServer(
    //    "Server=localhost,5437;Database=Bazaar;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=true");
});
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped(_ => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));
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

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ContractingDbContext>();
    await dbContext.Seed(scope.ServiceProvider);
}

app.Run();

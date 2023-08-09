using Catalog.Repositories;

var builder = WebApplication.CreateBuilder(args);

#region Register app services
builder.Services.AddDbContext<CatalogDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration["ConnectionString"]);
    //option.UseSqlServer("Server=localhost,5433;Database=Bazaar;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=true");
});
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped(sp => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var catalogContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await catalogContext.Seed(scope.ServiceProvider);
}

await app.RunAsync();

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
builder.Services.AddSingleton<LockManager<int>>();
builder.Services.AddSingleton<IResourceManager<CatalogItem, int>, CatalogTransactionalResourceManager>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    var lockManager = sp.GetRequiredService<LockManager<int>>();
    return new CatalogTransactionalResourceManager(scopeFactory, lockManager, item => item.Id);
});
builder.Services.AddSingleton<TransactionCoordinator>();
builder.Services.AddSingleton(sp => new HttpClient { Timeout = TimeSpan.FromSeconds(20) });
builder.Services.AddSingleton<IResourceLocationResolver, TransactionalUriResolver>();
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

using Bazaar.Catalog.Infrastructure.Transactional;
using Bazaar.Catalog.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ICatalogRepository, CatalogRepository>();
builder.Services.AddSingleton<JsonDataAdapter>();
builder.Services.AddSingleton<LockManager<int>>();
builder.Services.AddSingleton<IResourceManager<CatalogItem, int>, CatalogTransactionalResourceManager>(sp =>
{
    var catalogRepo = sp.GetRequiredService<ICatalogRepository>();
    var lockManager = sp.GetRequiredService<LockManager<int>>();
    return new CatalogTransactionalResourceManager(catalogRepo, lockManager, item => item.Id);
});
builder.Services.AddSingleton<TransactionCoordinator>();
builder.Services.AddSingleton(sp => new HttpClient { Timeout = TimeSpan.FromSeconds(20) });
builder.Services.AddSingleton<IResourceLocationResolver, TransactionalUriResolver>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();

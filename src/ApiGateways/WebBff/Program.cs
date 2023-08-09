var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register app services
builder.Services.AddScoped<IOrderRepository>(
    sp =>
    {
        var ordersUri = builder.Configuration["OrdersUri"]!;
        var httpClient = sp.GetRequiredService<HttpClient>();
        return new OrderRepository(ordersUri, httpClient);
    });

builder.Services.AddScoped<ICatalogRepository>(
    sp =>
    {
        var catalogUri = builder.Configuration["CatalogUri"]!;
        var httpClient = sp.GetRequiredService<HttpClient>();
        return new CatalogRepository(catalogUri, httpClient);
    });

builder.Services.AddScoped<IBasketRepository>(
    sp =>
    {
        var basketUri = builder.Configuration["BasketsUri"]!;
        var httpClient = sp.GetRequiredService<HttpClient>();
        var logger = sp.GetRequiredService<ILogger<BasketRepository>>();
        return new BasketRepository(basketUri, httpClient, logger);
    });

builder.Services.AddSingleton(sp => new HttpClient { Timeout = TimeSpan.FromSeconds(20) });

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

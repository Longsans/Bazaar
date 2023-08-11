var builder = WebApplication.CreateBuilder(args);

#region Register app services
builder.Services.AddDbContext<BasketDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
    //options.UseSqlServer(
    //    "Server=localhost,5434;Database=Bazaar;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=true");

    options.UseTriggers(triggerOptions =>
    {
        triggerOptions.AddTrigger<BasketItemChangeTrigger>();
    });
});
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped(sp => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));
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
    var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
    await dbContext.Seed(scope.ServiceProvider);
}

app.Run();

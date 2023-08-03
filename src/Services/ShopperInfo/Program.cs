var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Register app service
builder.Services
    .AddDbContext<ShopperInfoDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration["ConnectionString"]);
        //options.UseSqlServer(
        //    "Server=shopper-info-sql;Database=Bazaar;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=true");
    })
    .AddScoped<IShopperRepository, ShopperRepository>()
    .AddScoped(_ => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));
#endregion

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        var enumConverter = new JsonStringEnumConverter();
        options.JsonSerializerOptions.Converters.Add(enumConverter);
    });
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
    var dbContext = scope.ServiceProvider.GetRequiredService<ShopperInfoDbContext>();
    await dbContext.Seed(scope.ServiceProvider);
}

app.Run();

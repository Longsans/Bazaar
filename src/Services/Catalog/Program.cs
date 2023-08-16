using Bazaar.BuildingBlocks.EventBus;
using Catalog.Repositories;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

#region Register app services
builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
    //options.UseSqlServer("Server=localhost\\MSSQLSERVER01;Database=Bazaar;Trusted_Connection=True;TrustServerCertificate=True;");
});
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped(sp => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));
builder.Services.RegisterEventBus(builder.Configuration);
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

app.ConfigureEventBus();

await app.RunAsync();

public static class EventBusExtensionMethods
{
    public static void RegisterEventBus(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
            var factory = new ConnectionFactory()
            {
                HostName = config["EventBusConnection"],
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(config["EventBusUserName"]))
            {
                factory.UserName = config["EventBusUserName"];
            }

            if (!string.IsNullOrEmpty(config["EventBusPassword"]))
            {
                factory.Password = config["EventBusPassword"];
            }

            var retryCount = 5;
            if (!string.IsNullOrEmpty(config["EventBusRetryCount"]))
            {
                retryCount = int.Parse(config["EventBusRetryCount"]!);
            }

            return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
        });
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var subscriptionClientName = config["SubscriptionClientName"];
            var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            var retryCount = 5;
            if (!string.IsNullOrEmpty(config["EventBusRetryCount"]))
            {
                retryCount = int.Parse(config["EventBusRetryCount"]!);
            }

            return new EventBusRabbitMQ(
                rabbitMQPersistentConnection,
                logger,
                sp,
                eventBusSubcriptionsManager,
                subscriptionClientName,
                retryCount);
        });
        services.AddTransient<OrderCreatedIntegrationEventHandler>();
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
    }
}

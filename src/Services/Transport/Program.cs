using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Register application services
builder.Services.AddScoped<IEstimationService, BasicEstimationService>();
builder.Services.AddScoped<InventoryReturnProcessService>();
builder.Services.AddScoped<DeliveryProcessService>();
builder.Services.AddScoped<PickupProcessService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddDbContext<TransportDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

builder.Services.RegisterEventBus(builder.Configuration);
#endregion

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

app.ConfigureEventBus();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TransportDbContext>();
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.MigrateAsync();
}

app.Run();

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
        services.AddTransient<ProductFbbInventoryDeletedIntegrationEventHandler>();
        services.AddTransient<LotQuantitiesSentForReturnIntegrationEventHandler>();
        services.AddTransient<OrderStatusChangedToShippingIntegrationEventHandler>();
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.Subscribe<
            ProductFbbInventoryDeletedIntegrationEvent,
            ProductFbbInventoryDeletedIntegrationEventHandler>();
        eventBus.Subscribe<
            LotQuantitiesSentForReturnIntegrationEvent,
            LotQuantitiesSentForReturnIntegrationEventHandler>();
        eventBus.Subscribe<
            OrderStatusChangedToShippingIntegrationEvent,
            OrderStatusChangedToShippingIntegrationEventHandler>();
    }
}
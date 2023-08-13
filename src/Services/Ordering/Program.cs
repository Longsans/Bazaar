var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Register app services
builder.Services.AddDbContext<OrderingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
    //options.UseSqlServer(
    //    "Server=localhost,5435;Database=Bazaar;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=true");

    options.UseTriggers(triggerOptions =>
    {
        triggerOptions.AddTrigger<InsertOrderItemsTrigger>();
    });
});
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped(sp => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));
builder.Services.RegisterEventBus(builder.Configuration);
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

app.ConfigureEventBus();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
    await dbContext.Seed(scope.ServiceProvider);
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
        services.AddTransient<OrderPaymentSucceededIntegrationEventHandler>();
        services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
        services.AddTransient<OrderStocksConfirmedIntegrationEventHandler>();
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderPaymentSucceededIntegrationEvent, OrderPaymentSucceededIntegrationEventHandler>();
        eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();
    }
}
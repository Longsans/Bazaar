using Bazaar.BuildingBlocks.Transactions.Abstractions;
using Bazaar.Ordering.Infrastructure.Transactional;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<JsonDataAdapter>();
builder.Services.AddSingleton<LockManager<int>>();
builder.Services.AddSingleton<IResourceManager<Order, int>, OrderTransactionalResourceManager>(sp =>
{
    var orderRepo = sp.GetRequiredService<IOrderRepository>();
    var lockManager = sp.GetRequiredService<LockManager<int>>();
    return new OrderTransactionalResourceManager(orderRepo, lockManager, o => o.Id);
});
builder.Services.AddSingleton(sp => new HttpClient { Timeout = TimeSpan.FromSeconds(20) });
builder.Services.RegisterEventBus(builder.Configuration);

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

app.Run();

public static class EventBusExtensionMethods
{
    public static void RegisterEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]!);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                var subscriptionClientName = configuration["SubscriptionClientName"];
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]!);
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
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderPaymentSucceededIntegrationEvent, OrderPaymentSucceededIntegrationEventHandler>();
    }
}
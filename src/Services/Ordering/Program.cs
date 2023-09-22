var builder = WebApplication.CreateBuilder(args);
var IF_IDENTITY_ELSE = (Action doWithIdentity, Action doWithoutIdentity) =>
{
    if (string.IsNullOrWhiteSpace(builder.Configuration["DisableIdentity"]))
    {
        doWithIdentity();
    }
    else
    {
        doWithoutIdentity();
    }
};

// Add services to the container.
#region Register app services
builder.Services.AddDbContext<OrderingDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
    options.UseTriggers(triggerOptions =>
    {
        triggerOptions.AddTrigger<InsertOrderItemsTrigger>();
    });
});
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped(sp => new JsonDataAdapter(builder.Configuration["SeedDataFilePath"]!));
builder.Services.RegisterEventBus(builder.Configuration);

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityApi"];
        options.Audience = "ordering";
        options.RequireHttpsMetadata = false;
    });
builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("HasOrderingScope", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("scope", "ordering"));
});
#endregion

builder.Services.AddControllers();
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

// DISABLES IDENTITY
IF_IDENTITY_ELSE(
    () =>
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers().RequireAuthorization("HasOrderingScope");
    },
    () => app.MapControllers()
);

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

        services.AddTransient<OrderStocksInadequateIntegrationEventHandler>();
        services.AddTransient<OrderItemsUnavailableIntegrationEventHandler>();

        services.AddTransient<BuyerCheckoutAcceptedIntegrationEventHandler>();
        services.AddTransient<OrderStocksConfirmedIntegrationEventHandler>();
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderPaymentSucceededIntegrationEvent, OrderPaymentSucceededIntegrationEventHandler>();
        eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();

        eventBus.Subscribe<OrderItemsUnavailableIntegrationEvent, OrderItemsUnavailableIntegrationEventHandler>();
        eventBus.Subscribe<OrderStocksInadequateIntegrationEvent, OrderStocksInadequateIntegrationEventHandler>();

        eventBus.Subscribe<BuyerCheckoutAcceptedIntegrationEvent, BuyerCheckoutAcceptedIntegrationEventHandler>();
        eventBus.Subscribe<OrderStocksConfirmedIntegrationEvent, OrderStocksConfirmedIntegrationEventHandler>();
    }
}
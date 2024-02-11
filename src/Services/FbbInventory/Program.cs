var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Register application services
builder.Services.AddDbContext<FbbInventoryDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionString"]);
});

builder.Services.AddScoped<StockTransactionService>();
builder.Services.AddScoped<StockAdjustmentService>();
builder.Services.AddScoped<RemovalService>();
builder.Services.AddScoped<IStockInspectionService, FixedStockInspectionService>();
builder.Services.AddScoped<DeleteProductInventoryService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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
    var dbContext = scope.ServiceProvider.GetRequiredService<FbbInventoryDbContext>();
    await dbContext.Seed();
}

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
        //services.AddTransient<CatalogItemDeletedIntegrationEventHandler>();
        services.AddTransient<FbbInventoryPickedUpIntegrationEventHandler>();
        services.AddTransient<ProductFbbInventoryPickupsStatusChangedIntegrationEventHandler>();
        services.AddTransient<DisposalOrderCompletedIntegrationEventHandler>();
        services.AddTransient<DisposalOrderCancelledIntegrationEventHandler>();
        services.AddTransient<InventoryReturnCompletedIntegrationEventHandler>();
        services.AddTransient<InventoryReturnCancelledIntegrationEventHandler>();
        services.AddTransient<ProductListingClosedIntegrationEventHandler>();
        services.AddTransient<ProductRelistedIntegrationEventHandler>();
        services.AddTransient<FbbProductsOrderedIntegrationEventHandler>();
    }

    public static void ConfigureEventBus(this IApplicationBuilder app)
    {
        var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        //eventBus.Subscribe<
        //    CatalogItemDeletedIntegrationEvent,
        //    CatalogItemDeletedIntegrationEventHandler>();
        eventBus.Subscribe<
            FbbInventoryPickedUpIntegrationEvent,
            FbbInventoryPickedUpIntegrationEventHandler>();
        eventBus.Subscribe<
            ProductFbbInventoryPickupsStatusChangedIntegrationEvent,
            ProductFbbInventoryPickupsStatusChangedIntegrationEventHandler>();
        eventBus.Subscribe<
            DisposalOrderCompletedIntegrationEvent,
            DisposalOrderCompletedIntegrationEventHandler>();
        eventBus.Subscribe<
            DisposalOrderCancelledIntegrationEvent,
            DisposalOrderCancelledIntegrationEventHandler>();
        eventBus.Subscribe<
            InventoryReturnCompletedIntegrationEvent,
            InventoryReturnCompletedIntegrationEventHandler>();
        eventBus.Subscribe<
            InventoryReturnCancelledIntegrationEvent,
            InventoryReturnCancelledIntegrationEventHandler>();

        eventBus.Subscribe<
            ProductListingClosedIntegrationEvent,
            ProductListingClosedIntegrationEventHandler>();
        eventBus.Subscribe<
            ProductRelistedIntegrationEvent,
            ProductRelistedIntegrationEventHandler>();

        eventBus.Subscribe<
            FbbProductsOrderedIntegrationEvent,
            FbbProductsOrderedIntegrationEventHandler>();
    }
}